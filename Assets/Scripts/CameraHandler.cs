using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float zoomSpeed = 0.0125f;
    [SerializeField] float zoomSize = 8;
    [SerializeField] float originalSize = 10;

    [SerializeField] bool zoomActive = false;

    [SerializeField] Camera mainCam;
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;



    private void Update()
    {
        Vector3 cameraPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, cameraPos, moveSpeed);
    }

    private void LateUpdate()
    {
        Zoom();
    }

    private void Zoom()
    {
        if(zoomActive)
        {
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, zoomSize, zoomSpeed);
        }
        else
        {
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, originalSize, zoomSpeed);
        }
    }
}
