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
    [SerializeField] GameObject deathEffect;
    [SerializeField] float health = 100f;
    [SerializeField] float invisDuration;

    float currentHealth;
    bool isInvis = false;
    bool isDead = false;

    CameraShake cameraShake;

    private void Start()
    {
        cameraShake = FindObjectOfType<CameraShake>();

        currentHealth = health;
    }

    private void Update()
    {
        healthBarBG.fillAmount = Mathf.Lerp(healthBarBG.fillAmount, (currentHealth / health) - 0.01f, healthBarSlideSpeed);

        //För test
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            PlayerDamage(20);
        }
    }

    public void PlayerDamage(int damage)
    {
        if (!isInvis && !isDead)
        {
            currentHealth -= damage;
            healthBar.fillAmount = currentHealth / health;

            StartCoroutine(cameraShake.Shake(0.1f, 0.2f));
            StartCoroutine(InvisRoutine());

            health -= damage;

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        isDead = true;
        Destroy(gameObject);
    }

    private IEnumerator InvisRoutine()
    {
        isInvis = true;

        yield return new WaitForSeconds(invisDuration);

        isInvis = false;
    }
}