using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserLedObject : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer = null;
    [SerializeField] Transform laserStartingPos = null;
    [SerializeField] float maxLineLength = 100f;
    [SerializeField] float laserIncrease = 100f;

    private float laser = 0.01f;
    private Vector2 rayDirection;
    private float laserZRotation;
    private bool decreaseLaser = true;
    private bool transformLaserCalled = false;

    private void Update()
    {
        laserZRotation = laserStartingPos.eulerAngles.z;
        rayDirection = new Vector2(Mathf.Cos(laserZRotation * Mathf.Deg2Rad), Mathf.Sin(laserZRotation * Mathf.Deg2Rad));

        // If TransformLaser() has not been called, set decreaseLaser to true
        if (!transformLaserCalled)
        {
            decreaseLaser = true;
        }

        if (decreaseLaser)
        {
            // Reduce the laser length
            laser -= laserIncrease * Time.deltaTime;
            if (laser < 0.01)
            {
                laser = 0.01f;
                decreaseLaser = false; // Stop decreasing
            }
            lineRenderer.SetPosition(1, new Vector3(laser, 0, 0));
        }

        // Reset transformLaserCalled flag at the end of Update
        transformLaserCalled = false;
    }

    public void TransformLaser()
    {
        decreaseLaser = false; // Set decreaseLaser to false when TransformLaser is called
        RaycastHit2D hit = Physics2D.Raycast(laserStartingPos.position, rayDirection * maxLineLength);
        Debug.DrawRay(laserStartingPos.position, rayDirection * hit.distance);

        if (hit)
        {
            lineRenderer.SetPosition(1, new Vector3(hit.distance * 2.0f, 0, 0));
            laser = hit.distance;
        }
        else
        {
            if (laser < maxLineLength)
            {
                laser += laserIncrease * Time.deltaTime;
                lineRenderer.SetPosition(1, new Vector3(laser, 0, 0));
            }
        }

        // Set transformLaserCalled flag to true when TransformLaser is called
        transformLaserCalled = true;
    }
}
