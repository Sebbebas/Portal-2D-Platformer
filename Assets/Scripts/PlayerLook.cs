using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // Configurable Parameters
    [SerializeField] Camera cam;
    [SerializeField] float camSpeed = 5f;
    [SerializeField] float rangeMultiplier = 3f;
    [SerializeField] float radius = 5f;

    // Cached References
    Rigidbody2D cameraRigidbody;

    private void Start()
    {
        cameraRigidbody = cam.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = mousePosition - (Vector2)transform.position;

        // Move the camera towards the mouse
        Vector2 newPosition = (Vector2)transform.position + directionToMouse * camSpeed * Time.deltaTime * rangeMultiplier;

        // Clamp the camera position within the radius
        Vector2 directionToNewPosition = newPosition - (Vector2)transform.position;
        if (directionToNewPosition.magnitude > radius)
        {
            newPosition = (Vector2)transform.position + directionToNewPosition.normalized * radius;
        }

        // Move the camera to the new position
        cameraRigidbody.MovePosition(newPosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (cam.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
