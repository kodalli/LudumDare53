using System.Collections;
using System.Collections.Generic;
using TNS.InputMiddlewareSystem;
using UnityEngine;
using UnityEngine.Events;

public class TroopMovement : MonoBehaviour
{
    // rectangle drawn to show selection area
    [SerializeField] private Transform selectionArea;
    // cursor gameObject
    [SerializeField] private Transform rightClick;
    [SerializeField] private InputProvider inputProvider;
    private InputState m_InputState;
    private Vector2 m_MovementDirection;
    private Vector2 m_StartPosition;
    private List<IUnitRts> m_SelectedUnitRtsList;
    private readonly Collider2D[] m_Collider2Ds = new Collider2D[20];
    private Camera m_Camera;
    private bool m_IsSelectingTroops;

    private Vector2 MouseWorldPosition
    {
        get
        {
            System.Diagnostics.Debug.Assert(Camera.main != null, "Camera.main != null");
            return Camera.main.ScreenToWorldPoint(m_InputState.MouseDirection);
        }
    }


    private void OnEnable()
    {
        // Actions
        inputProvider.OnLeftClickAction += StartSelection;
        inputProvider.OnLeftClickReleaseAction += ReleaseSelection;
        inputProvider.OnRightClickAction += MoveToSelected;

        m_SelectedUnitRtsList = new List<IUnitRts>();
        selectionArea.gameObject.SetActive(false);
        m_Camera = Camera.main;
        m_IsSelectingTroops = false;
    }

    private void OnDisable()
    {
        inputProvider.OnLeftClickAction -= StartSelection;
        inputProvider.OnLeftClickReleaseAction -= ReleaseSelection;
        inputProvider.OnRightClickAction -= MoveToSelected;
    }

    private void Update()
    {
        m_InputState = inputProvider.GetState();

        // Only draw area if initial left click pressed to start selection
        if (m_IsSelectingTroops)
        {
            DrawSelectionArea();
        }
    }

    public void HandleTroopMovement()
    {
        // Move troops after right click is released
        MoveToSelected();

        // Move cursor to click location
        // TODO: do click animation
        rightClick.transform.position = MouseWorldPosition;
        Debug.Log($"Mouse world position: {MouseWorldPosition}");
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
        Debug.Log("LeftClick!");
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
        foreach (var unit in m_SelectedUnitRtsList)
        {
            unit.ToggleSelection(false);
        }

        Debug.Log($"{size} selected");
        m_SelectedUnitRtsList.Clear();
        while (--size >= 0)
        {
            var unitRts = m_Collider2Ds[size].GetComponent<IUnitRts>();
            if (unitRts == null) continue;
            unitRts.ToggleSelection(true);
            m_SelectedUnitRtsList.Add(unitRts);
        }
    }

    private void MoveToSelected()
    {
        foreach (var units in m_SelectedUnitRtsList)
        {
            units.MoveToPosition(MouseWorldPosition);
        }

    }

}
