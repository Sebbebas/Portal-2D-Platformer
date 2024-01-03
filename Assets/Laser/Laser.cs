using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public enum LaserMode
    {
        Still,
        Rotate,
        Moving,
        MovedByInteracion,
        Total
    }

    [SerializeField] LineRenderer lineRenderer = null;
    [SerializeField] Transform laserStartPoint = null;
    [SerializeField] float maxLineLength = 50f;
    [SerializeField] float laserIncrease = 100f;
    [SerializeField] GameObject startVFX;
    [SerializeField] GameObject endVFX;

    private float currentMaxLineLength = 0.01f;

    public LaserMode laserMode = LaserMode.Total;


    [Header("Rotate Mode values")]
    [SerializeField] float switchTime = 2.0f; // Time to switch between two angles
    [SerializeField] float angleA = 0.0f;
    [SerializeField] float angleB = 180.0f;

    //Move
    [Header("Moving Mode values")]
    [SerializeField] Vector2 pointA = new Vector2(-1, 0);
    [SerializeField] Vector2 pointB = new Vector2(1, 0);
    [SerializeField] float timeFromAToB = 1f;
    [SerializeField] float timeToPauseOnPoint = 0.2f;

    private bool towardsPointA;
    private Vector2 localPointA;
    private Vector2 localPointB;
    private float laserZRotation;
    private float timer = 0.0f;
    private bool towardsValueA;

    //Reflect
    List<Vector3> Points;
    int maxReflections = 10;
    List<float> reflectionLength;

    bool moveLaser = false;
    public Vector2 mousePos; //This variable is manipulated in InteractionController

    void Start()
    {
        reflectionLength = Enumerable.Repeat(0.01f, maxReflections + 1).ToList();
        Points = new List<Vector3>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, laserStartPoint.position);
        lineRenderer.SetPosition(1, laserStartPoint.position);

        if (laserMode == LaserMode.Moving)
        {
            localPointA = (Vector2)transform.position + pointA;
            localPointB = (Vector2)transform.position + pointB;

            StartCoroutine(MoveFromAToBRoutine());
        }
    }

    public void ManuallyMoveLaser()
    {
        moveLaser = !moveLaser;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rayDirection = laserStartPoint.right.normalized;
        FindCollision(rayDirection);

        if (laserMode == LaserMode.Rotate)
        {
            RotateZBackAndForth();
        }

        if (laserMode == LaserMode.MovedByInteracion)
        {
            if (moveLaser)
            {
                Debug.Log(mousePos);
                Rotate();
            }
        }

        lineRenderer.positionCount = Points.Count;
        lineRenderer.SetPositions(Points.ToArray());
    }

    void Rotate()
    {
        if (mousePos != Vector2.zero)
        {
            // Calculate the angle in radians
            float angleRadians = Mathf.Atan2(mousePos.y, mousePos.x);

            // Convert the angle to degrees
            float angleDegrees = angleRadians * Mathf.Rad2Deg;

            // Create a Quaternion rotation around the Z-axis based on the angle
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angleDegrees);

            // Apply the rotation to the transform
            transform.rotation = targetRotation;
        }
    }

    private void FindCollision(Vector3 rayDirection)
    {
        if (currentMaxLineLength < maxLineLength)
        {
            currentMaxLineLength += laserIncrease * Time.deltaTime;
            currentMaxLineLength = Mathf.Min(currentMaxLineLength, maxLineLength);
        }

        Points.Clear();
        Points.Add(laserStartPoint.position);

        int reflections = 1;
        Vector3 origin = laserStartPoint.position;

        while (reflections < maxReflections)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, currentMaxLineLength);

            if (hit)
            {
                FindLaserLedBlock(hit);
                Points.Add(hit.point);

                if (hit.collider.CompareTag("Mirror"))
                {
                    rayDirection = Vector3.Reflect(rayDirection, hit.normal);
                    origin = (Vector3)hit.point + (rayDirection.normalized * 0.01f); // Small offset to avoid self-collision

                    // Reflect further from the mirror
                    ReflectFurther(origin, rayDirection, reflections, hit.point);

                    // After reflection, smoothly increase currentMaxLineLength towards maxLineLength
                    currentMaxLineLength = hit.distance;

                    return; // Exit loop after reflection
                }
                else
                {
                    if (currentMaxLineLength < hit.distance)
                    {
                        // Smoothly increase currentMaxLineLength towards the distance
                        currentMaxLineLength = Mathf.MoveTowards(currentMaxLineLength, hit.distance, Time.deltaTime * laserIncrease);
                    }
                    else
                    {
                        // Set currentMaxLineLength to the distance
                        currentMaxLineLength = hit.distance;
                    }

                    // Reset the reflected length to the original value (0.01f) when not colliding with a mirror
                    ResetReflectionLength();
                    Points.Add(origin + rayDirection * currentMaxLineLength);
                    break;
                }
            }
            else
            {
                // Reset the reflected length to the original value (0.01f) when no hit detected
                ResetReflectionLength();
                Points.Add(origin + rayDirection * currentMaxLineLength);
                break; // Exit loop if no hit detected
            }
        }
    }

    private void ReflectFurther(Vector3 origin, Vector3 newDirection, int reflections, Vector3 hitPoint)
    {
        if (reflections >= maxReflections || reflections >= reflectionLength.Count)
            return;

        if (reflectionLength[reflections] < maxLineLength)
        {
            reflectionLength[reflections] += laserIncrease * Time.deltaTime;
            reflectionLength[reflections] = Mathf.Min(reflectionLength[reflections], maxLineLength);
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, newDirection, reflectionLength[reflections]);

        if (hit)
        {
            FindLaserLedBlock(hit);

            if (hit.collider.CompareTag("Mirror"))
            {
                newDirection = Vector3.Reflect(newDirection, hit.normal);
                var neworigin = (Vector3)hit.point + (newDirection.normalized * 0.01f); // Small offset to avoid self-collision

                Points.Add(hit.point); // Add hit point before reflecting

                ReflectFurther(neworigin, newDirection, reflections + 1, hit.point); // Recursive reflection
                reflectionLength[reflections] = hit.distance;
            }
            else
            {
                reflectionLength[reflections] = hit.distance;
                Points.Add(hit.point); // Add hit point
                return; // Exit the recursion after adding one point
            }
        }
        else
        {
            Points.Add(hitPoint + newDirection * reflectionLength[reflections]); // Use the given reflection's length

            // Clear or set the remaining reflections beyond the current one to 0.01f
            for (int i = reflections + 1; i < reflectionLength.Count; i++)
            {
                reflectionLength[i] = 0.01f;
            }
        }
    }

    private void ResetReflectionLength()
    {
        for (int i = 0; i < reflectionLength.Count; i++)
        {
            reflectionLength[i] = 0.01f; // Reset each value to the initial length
        }
    }

    private void FindLaserLedBlock(RaycastHit2D block)
    {
        if (block.collider.CompareTag("LaserBlockSides"))
        {
            Transform parentTransform = block.collider.transform.parent;
            if (parentTransform != null)
            {
                LaserLedObject laserObject = parentTransform.GetComponent<LaserLedObject>();
                if (laserObject != null)
                {
                    laserObject.TransformLaser();
                }
            }
        }
    }

    //Rotate
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
            laserZRotation = Mathf.Lerp(angleA, angleB, timer / switchTime);
        }
        else
        {
            laserZRotation = Mathf.Lerp(angleB, angleA, timer / switchTime);
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, laserZRotation);
    }

    //Move
    IEnumerator MoveFromAToBRoutine()
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
        if (laserMode == LaserMode.Rotate)
        {
            Vector3 pointOnePosition = transform.position + Quaternion.Euler(0, 0, angleA) * Vector3.right;
            Vector3 pointTwoPosition = transform.position + Quaternion.Euler(0, 0, angleB) * Vector3.right;

            Gizmos.color = Color.green; // Change the color if desired
            Gizmos.DrawLine(transform.position, pointOnePosition);
            Gizmos.DrawLine(transform.position, pointTwoPosition);
        }

        //MoveGizmos
        if (laserMode == LaserMode.Moving)
        {
            Vector2 myPosition = transform.position;

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(myPosition + pointA, 0.3f);
            Gizmos.DrawWireSphere(myPosition + pointB, 0.3f);

            Gizmos.color = Color.white;

            Gizmos.DrawLine(myPosition + pointA, myPosition + pointB);
        }
    }
}
