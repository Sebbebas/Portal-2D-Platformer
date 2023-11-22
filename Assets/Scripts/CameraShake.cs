using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float magnitude, float duration)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float shakeX = transform.position.x + Random.Range(-1f, 1f) * magnitude;
            float shakeY = transform.position.y + Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(shakeX, shakeY, transform.position.z);
            elapsed += Time.deltaTime;

            yield return 0;
        }

        transform.position = originalPosition;
    }
}
