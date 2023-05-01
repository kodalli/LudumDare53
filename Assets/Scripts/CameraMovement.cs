using TNS.InputMiddlewareSystem;
using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    public float speed = 10f;
    public float scrollSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (mousePosition.x < 0 || mousePosition.x > Screen.width || mousePosition.y < 0 || mousePosition.y > Screen.height)
        {
            return;
        }
        if (mousePosition.x < screenPosition.x - Screen.width / 2f)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        else if (mousePosition.x > screenPosition.x + Screen.width / 2f)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        if (mousePosition.y < screenPosition.y - Screen.height / 2f)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
        else if (mousePosition.y > screenPosition.y + Screen.height / 2f)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * scrollSpeed;
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, minZoom, maxZoom),
            transform.position.z
        );
    }
}