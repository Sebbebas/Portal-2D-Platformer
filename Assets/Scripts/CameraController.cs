using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float zoomSpeed = 0.0125f;
    [SerializeField] float zoomSize = 8;
    [SerializeField] float originalSize = 10;

    [HideInInspector] public bool zoomActive;

    [Space]

    [SerializeField] Camera mainCam;
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;

    [Header("Cursor")]
    [SerializeField] Texture2D mouseCursorTexture;
    [SerializeField] Vector2 mouseCursorOffset;

    private bool moveToCamera;

    private void Start()
    {
        mainCam = FindObjectOfType<Camera>();
        player = FindObjectOfType<PlayerMove>().transform;

        Cursor.SetCursor(mouseCursorTexture, mouseCursorOffset, CursorMode.ForceSoftware);
    }

    private void Update()
    {
        Vector3 cameraPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, cameraPos, moveSpeed);
    }

    private void LateUpdate()
    {
        Zoom();
        MoveCamToMouse();
    }

    private void MoveCamToMouse()
    {
        if (!moveToCamera) { return; }
        Vector3 camPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        //Vector2.Lerp(transform.position.x, camPos.x, 1f);
    }

    private void Zoom()
    {
        if (zoomActive)
        {
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, zoomSize, zoomSpeed);
        }
        else
        {
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, originalSize, zoomSpeed);
        }
    }

    public void SetCameraToMouse(bool moveToMouse)
    {
        if (moveToMouse) { moveToCamera = true; } else { moveToCamera = false; }
    }

    public bool GetCameraToMouseBool()
    {
        return moveToCamera;
    }

    /*public bool GetZoomActive()
    {
        return zoomActive;
    }*/

    private void OnDrawGizmosSelected()
    {
        Vector3 nextCamPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(player.position, nextCamPos);
    }
}
