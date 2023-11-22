using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("HealthBar")]
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarBG;
    [Space]
    [SerializeField] float healthBarSlideSpeed = 0.005f;
    [Header("Player")]
    [SerializeField] float maxHealth = 5.0f;
    [SerializeField] float invisDuration;

    float currentHealth;
    bool isInvis = false;
    bool isDead = false;

    CameraShake cameraShake;

    private void Start()
    {
        cameraShake = FindObjectOfType<CameraShake>();

        currentHealth = maxHealth;
    }

    private void Update()
    {
        healthBarBG.fillAmount = Mathf.Lerp(healthBarBG.fillAmount, (currentHealth / maxHealth) - 0.01f, healthBarSlideSpeed);

        //För test
        if (Input.GetKeyDown(KeyCode.Escape)) { PlayerDamage(1); }
    }

    public void PlayerDamage(int damage)
    {
        if (!isInvis && !isDead)
        {
            currentHealth -= damage;
            healthBar.fillAmount = currentHealth / maxHealth;

            StartCoroutine(cameraShake.Shake(0.1f, 0.2f));
            StartCoroutine(InvisRoutine());

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Aj Pappa");
        isDead = true;
    }

    private IEnumerator InvisRoutine()
    {
        isInvis = true;

        yield return new WaitForSeconds(invisDuration);

        isInvis = false;
    }
}
