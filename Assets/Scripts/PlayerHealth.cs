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
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float invisDuration = 0.5f;

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
        healthBarBG.fillAmount = Mathf.Lerp(healthBarBG.fillAmount, healthBar.fillAmount - 0.01f, healthBarSlideSpeed);

        //För test
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            PlayerDamage(20);
            //StartCoroutine(FindObjectOfType<PlayerMove>().Knockback(new Vector2(50f, 10f), 0f, 0.3f));
        }
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
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        isDead = true;
        Debug.Log("Player is dead");
    }

    private IEnumerator InvisRoutine()
    {
        isInvis = true;

        yield return new WaitForSeconds(invisDuration);

        isInvis = false;
    }
}