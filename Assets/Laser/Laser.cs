using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer = null;
    [SerializeField] Transform laserStartPoint = null;
    [SerializeField] bool visibleRay = true;
    [SerializeField] float maxLineLength = 50f;
    [SerializeField] float laserIncrease = 20f;

    private RaycastHit2D toHit;
    private Vector2 rayDirection;
    public float laser;


    // Start is called before the first frame update
    void Start()
    {
        laser = 0.002f * 5;
        lineRenderer.SetPosition(1, new Vector3(laser, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        FindCollision();
    }

    private void FindCollision()
    {
        float parentZRotation = transform.eulerAngles.z;
        rayDirection = new Vector2(Mathf.Cos(parentZRotation * Mathf.Deg2Rad), Mathf.Sin(parentZRotation * Mathf.Deg2Rad));
        RaycastHit2D hit = Physics2D.Raycast(laserStartPoint.position, rayDirection * maxLineLength);
        
        if (visibleRay) { Debug.DrawRay(laserStartPoint.position, rayDirection * maxLineLength); }

        if (hit)
        {
            lineRenderer.SetPosition(1, new Vector3(hit.distance, 0, 0));
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
    }

}
