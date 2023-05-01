using System.Collections;
using System.Collections.Generic;
using TNS.InputMiddlewareSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TroopMovement : MonoBehaviour
{
    // rectangle drawn to show selection area
    [SerializeField] private Transform selectionArea;

    // cursor gameObject
    [SerializeField] private Transform rightClick;
    [SerializeField] private InputProvider inputProvider;
    [SerializeField] private GameObject littleDogTroop;
    [SerializeField] private GameObject buffDogTroop;
    [SerializeField] private int balance = 100; //currency total
    private InputState m_InputState;
    private Vector2 m_MovementDirection;
    private Vector2 m_StartPosition;
    private List<IUnitRts> m_SelectedUnitRtsList;
    private readonly Collider2D[] m_Collider2Ds = new Collider2D[20];
    private Camera m_Camera;
    private bool m_IsSelectingTroops;
    private bool holdshift;

    private Vector2 MouseWorldPosition
    {
        get
        {
            System.Diagnostics.Debug.Assert(Camera.main != null, "Camera.main != null");
            return Camera.main.ScreenToWorldPoint(m_InputState.MouseDirection);
        }
    }

    public static Texture2D SpriteToTexture2D(GameObject obj)
    {
        var sprite = obj.GetComponent<SpriteRenderer>().sprite;
        if (sprite.rect.width != sprite.texture.width || sprite.rect.height != sprite.texture.height) {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else {
            return sprite.texture;
        }
    }

    private void OnEnable()
    {
        // Actions
        inputProvider.OnLeftClickPressedAction += StartSelection;
        inputProvider.OnLeftClickReleasedAction += ReleaseSelection;
        inputProvider.OnRightClickAction += HandleTroopMovement;
        // TODO: change to purchase with treats
        // inputProvider.OnJump += purchaseUnit;
        App.GameManager.PurchaseLittleDogEvent += () =>
        {
            if (App.GameManager.ConsumeDogTreats(2)) {
                SpawnTroop(littleDogTroop);
            }
            else {
                Debug.Log("Not Enough DogTreats !!! Need at least 2");
            }
        };
        App.GameManager.PurchaseBuffDogEvent  += () =>
        {
            if (App.GameManager.ConsumeDogTreats(5)) {
                SpawnTroop(buffDogTroop);
            }
            else
            {
                Debug.Log("Not Enough DogTreats !!! Need at least 5");
            }
        };;


        m_SelectedUnitRtsList = new List<IUnitRts>();
        selectionArea.gameObject.SetActive(false);
        m_Camera = Camera.main;
        m_IsSelectingTroops = false;
        Cursor.SetCursor(SpriteToTexture2D(rightClick.gameObject), Vector2.zero, CursorMode.Auto);
    }

    private void OnDisable()
    {
        inputProvider.OnLeftClickPressedAction -= StartSelection;
        inputProvider.OnLeftClickReleasedAction -= ReleaseSelection;
        inputProvider.OnRightClickAction -= HandleTroopMovement;
        // inputProvider.OnJump -= purchaseUnit;
    }

    private void Update()
    {
        holdshift = Input.GetKey(KeyCode.LeftShift);
        m_InputState = inputProvider.GetState();

        // Only draw area if initial left click pressed to start selection
        if (m_IsSelectingTroops) {
            DrawSelectionArea();
        }

        rightClick.transform.position = inputProvider.GetState().MouseDirection;
    }

    // private void PurchaseUnit()
    // {
    //     if (balance - 100 >= 0) {
    //         balance -= 100;
    //         SpawnTroop(troop);
    //     }
    // }
    //
    private void SpawnTroop(GameObject troop)
    {
        GameObject.Instantiate(troop);
    }

    public void HandleTroopMovement()
    {
        // Move troops after right click is released
        MoveToSelected();

        // Move cursor to click location
        // TODO: do click animation
        rightClick.transform.position = MouseWorldPosition;
        // Debug.Log($"Mouse world position: {MouseWorldPosition}");
    }

    private void DrawSelectionArea()
    {
        // Change selection area
        var currentMousePosition = MouseWorldPosition;
        var lowerLeft = new Vector2(Mathf.Min(m_StartPosition.x, currentMousePosition.x),
            Mathf.Min(m_StartPosition.y, currentMousePosition.y));
        var upperRight = new Vector2(Mathf.Max(m_StartPosition.x, currentMousePosition.x),
            Mathf.Max(m_StartPosition.y, currentMousePosition.y));
        selectionArea.position = lowerLeft;
        selectionArea.localScale = upperRight - lowerLeft;
    }

    private void StartSelection()
    {
        m_IsSelectingTroops = true;
        m_StartPosition = MouseWorldPosition;
        selectionArea.gameObject.SetActive(true);
    }

    private void ReleaseSelection()
    {
        m_IsSelectingTroops = false;
        selectionArea.gameObject.SetActive(false);
        var size = Physics2D.OverlapAreaNonAlloc(m_StartPosition, MouseWorldPosition, m_Collider2Ds);
        // Deselect
        //Debug.Log($"{size} selected");

        if (!holdshift) {
            foreach (var unit in m_SelectedUnitRtsList) {
                unit.ToggleSelection(false);
            }

            m_SelectedUnitRtsList.Clear();
        }


        // Debug.Log($"{size} selected");
        while (--size >= 0) {
            var unitRts = m_Collider2Ds[size].GetComponent<IUnitRts>();
            if (unitRts == null) continue;
            unitRts.ToggleSelection(true);
            m_SelectedUnitRtsList.Add(unitRts);
        }
    }

    private void MoveToSelected()
    {
        var targetPositionList =
            GetPositionListAround(MouseWorldPosition, new float[] { 1.5f, 3f, 6f }, new int[] { 5, 10, 20 });
        // var targetPositionList = GetPositionListAround(MouseWorldPosition, 1f, m_SelectedUnitRtsList.Count);
        var idx = 0;
        foreach (var units in m_SelectedUnitRtsList) {
            // Debug.Log($"index {idx}, target: {targetPositionList[idx]}");
            units.MoveToPosition(targetPositionList[idx]);
            idx = (idx + 1) % targetPositionList.Count;
        }
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray,
        int[] ringPositionCountArray)
    {
        var positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (var i = 0; i < ringDistanceArray.Length; ++i) {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i],
                ringPositionCountArray[i]));
        }

        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        var positionList = new List<Vector3>();
        for (var i = 0; i < positionCount; ++i) {
            var angle = i * (360f / positionCount);
            var dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            var position = startPosition + dir * distance;
            positionList.Add(position);
        }

        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}