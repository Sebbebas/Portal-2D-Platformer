using Unity.VisualScripting;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // Configurable Parameters
    [Header("Move Camera")]
    [SerializeField] Camera mainCamera;
    [SerializeField, Tooltip("How fast the camera moves towards the mouse")] float cameraSpeed = 5f;
    [SerializeField, Tooltip("How far away from the player the camera can move")] float radius = 5f;
    [SerializeField, Tooltip("Moves the camera towards the player")] float cameraReturnSpeed = 5f;
    [SerializeField, Tooltip("Moves the camera towards the mouse")] bool cameraToMouse = true;

    // Cached References
    Rigidbody2D cameraRigidbody;

    private void Start()
    {
        cameraRigidbody = mainCamera.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (cameraToMouse) { MoveCamera(); return; }
        else{ cameraRigidbody.MovePosition(new Vector2(Mathf.Lerp(mainCamera.transform.position.x, transform.position.x, cameraReturnSpeed * Time.deltaTime),Mathf.Lerp(mainCamera.transform.position.y, transform.position.y, cameraReturnSpeed * Time.deltaTime))); return; }
    }

    private void MoveCamera()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = mousePosition - (Vector2)transform.position;

        // Move the mainCamera towards the mouse
        Vector2 newPosition = (Vector2)transform.position + directionToMouse * cameraSpeed * Time.deltaTime;

        // Clamp the mainCamera position within the radius
        Vector2 directionToNewPosition = newPosition - (Vector2)transform.position;
        if (directionToNewPosition.magnitude > radius)
        {
            newPosition = (Vector2)transform.position + directionToNewPosition.normalized * radius;
        }

        // Move the mainCamera to the new position
        cameraRigidbody.MovePosition(newPosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (mainCamera == null) return;

        Gizmos.color = Color.red;
        Vector3 cameraDirection = transform.position - mainCamera.transform.position;
        Gizmos.DrawRay(mainCamera.transform.position, cameraDirection);
    }
}
