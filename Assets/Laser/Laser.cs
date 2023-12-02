using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaserMode
{
    Still,
    Rotate,
    Moving,
    Total
}
public class Laser : MonoBehaviour
{
    public LaserMode laserMode = LaserMode.Total;
    private bool[] laserModeList = new bool[(int)LaserMode.Total];

    [SerializeField] LineRenderer lineRenderer = null;
    [SerializeField] Transform laserStartPoint = null;
    [SerializeField] float maxLineLength = 50f;
    [SerializeField] float laserIncrease = 20f;

    [Header("Rotate Mode values")]
    [SerializeField] float switchTime = 2.0f; // Time to switch between two angles
    [SerializeField] float pointOne = 0.0f;
    [SerializeField] float pointTwo = 180.0f;
    
    //Move
    [Header("Moving Mode values")]
    [SerializeField] Vector2 pointA = new Vector2(0, -1);
    [SerializeField] Vector2 pointB = new Vector2(0, 1);
    [SerializeField] float timeFromAToB = 1f;
    [SerializeField] float timeToPauseOnPoint = 0.2f;

    private bool towardsPointA;

    private Vector2 localPointA;
    private Vector2 localPointB;
    //Move

    private Vector2 rayDirection;
    private float laser;
    private float laserZRotation;
    private float timer = 0.0f;
    private bool towardsValueA;

    // Start is called before the first frame update
    void Start()
    {
        laserModeList[(int)laserMode] = true;

        laser = 0.002f * 5;
        lineRenderer.SetPosition(1, new Vector3(laser, 0, 0));

        if (laserModeList[(int)LaserMode.Moving])
        {
            localPointA = (Vector2)transform.position + pointA;
            localPointB = (Vector2)transform.position + pointB;

            StartCoroutine(UpAndDownRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        laserZRotation = transform.eulerAngles.z;
        rayDirection = new Vector2(Mathf.Cos(laserZRotation * Mathf.Deg2Rad), Mathf.Sin(laserZRotation * Mathf.Deg2Rad));
        FindCollision();

        if (laserModeList[(int)LaserMode.Rotate])
        {
            RotateZBackAndForth();
        }
    }

    private void RotateZBackAndForth()
    {
        timer += Time.deltaTime;
        if (timer >= switchTime)
        {
            timer -= switchTime;
            towardsValueA = !towardsValueA; // Toggle between two angles
        }

        if (towardsValueA)
        {
            laserZRotation = Mathf.Lerp(pointOne, pointTwo, timer / switchTime);
        }
        else
        {
            laserZRotation = Mathf.Lerp(pointTwo, pointOne, timer / switchTime);
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, laserZRotation);
    }
    private void FindCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(laserStartPoint.position, rayDirection * maxLineLength);
        
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

    //Move
    IEnumerator UpAndDownRoutine()
    {
        while (true)
        {
            towardsPointA = !towardsPointA;
            float progress = 0;

            while (progress < 1)
            {
                float smoothProgress = Mathf.Pow(progress, 2) * (3f - 2f * progress);

                if (towardsPointA)
                {
                    transform.position = Vector2.Lerp(localPointB, localPointA, smoothProgress);
                }
                else
                {
                    transform.position = Vector2.Lerp(localPointA, localPointB, smoothProgress);
                }

                progress += Time.deltaTime / timeFromAToB;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(timeToPauseOnPoint);
        }
    }

    private void OnDrawGizmosSelected()
    {
        //RotateGizmos
        Vector3 pointOnePosition = transform.position + Quaternion.Euler(0, 0, pointOne) * Vector3.right;
        Vector3 pointTwoPosition = transform.position + Quaternion.Euler(0, 0, pointTwo) * Vector3.right;

        Gizmos.color = Color.green; // Change the color if desired
        Gizmos.DrawLine(transform.position, pointOnePosition);
        Gizmos.DrawLine(transform.position, pointTwoPosition);


        //MoveGizmos
        Vector2 myPosition = transform.position;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(myPosition + pointA, 0.3f);
        Gizmos.DrawWireSphere(myPosition + pointB, 0.3f);

        Gizmos.color = Color.white;

        Gizmos.DrawLine(myPosition + pointA, myPosition + pointB);
    }
}
