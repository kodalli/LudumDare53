using System.Collections;
using System.Collections.Generic;
using TNS.InputMiddlewareSystem;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float scrollSpeed = 2.0f;
    public float minY = 5.0f;
    public float maxY = 80.0f;

    private Vector3 _initialPosition;
    [SerializeField] private InputProvider inputProvider;

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
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Zoom in and out using the mouse scroll wheel
        float newY = Mathf.Clamp(transform.position.y - (scrollInput * scrollSpeed), minY, maxY);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Reset camera position with 'R' key
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = _initialPosition;
        }
    }
}
