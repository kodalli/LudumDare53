using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TNS.InputMiddlewareSystem;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float scrollSpeed = 2.0f;
    public float minY = -1000f;
    public float maxY = 1000f;
    public float maxX = 1000f;
    public float minX = -1000f;
    public float minZoom = 5f;
    public float maxZoom = 10f;

    private Vector3 _initialPosition;
    [SerializeField] private InputProvider inputProvider;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    void Start()
    {
        _initialPosition = transform.position;
    }

    void Update()
    {
        var state = inputProvider.GetState();
        float horizontalInput = state.MovementDirection.x;
        float verticalInput = state.MovementDirection.y;
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0);
        var t = cinemachineVirtualCamera.transform;
        t.position += moveDirection * moveSpeed * Time.deltaTime;

        float newY = Mathf.Clamp(t.position.y, minY, maxY);
        float newX = Mathf.Clamp(t.position.x, minX, maxX);
        t.position = new Vector3(newX, newY, t.position.z);

        if (scrollInput != 0f)
        {
            var cur = cinemachineVirtualCamera.m_Lens.OrthographicSize;
            cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(cur - (scrollInput * scrollSpeed), minZoom, maxZoom);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            t.position = _initialPosition;
        }
    }
}
