using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] SpriteRenderer player;
    [SerializeField] GameObject deathEffect;
    [SerializeField] float maxHealth = 100f;

    [Header("HealthBar")]
    [SerializeField, Tooltip("Health Left")] Image healthBar;
    [SerializeField, Tooltip("Health Lost")] Image lostHealthBar;
    [SerializeField] float slideSpeed = 0.005f;

    [Header("Invisible")]
    [SerializeField] float invisDuration = 1f;
    [SerializeField] float blinkRate = 0.1f;

    float currentHealth;
    bool isInvis = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        HealthBarUpdate();
        StartCoroutine(Blinking());

        //För test
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            PlayerDamage(20);
            StartCoroutine(FindObjectOfType<PlayerMove>().Knockback(new Vector2(0f, 2f), 0f, 0.2f));
        }
    }

    public void PlayerDamage(int damage)
    {
        if (!isInvis)
        {
            CameraShake cameraShake = FindObjectOfType<CameraShake>();

            currentHealth -= damage;

            StartCoroutine(cameraShake.Shake(0.1f, 0.2f));
            StartCoroutine(InvisRoutine());

            if (currentHealth <= 0)
            {
                StartCoroutine(Die(1f));
            }
        }
    }
    private void HealthBarUpdate()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
        lostHealthBar.fillAmount = Mathf.Lerp(lostHealthBar.fillAmount, healthBar.fillAmount - 0.01f, slideSpeed);
    }
  
    public IEnumerator Die(float time)
    {
        CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();

        yield return new WaitForSeconds(time);
        transform.position = checkpointManager.GetSpawnPoint();

        currentHealth = maxHealth;
        lostHealthBar.fillAmount = 1f;
    }

    private IEnumerator InvisRoutine()
    {
        isInvis = true;
        yield return new WaitForSeconds(invisDuration);
        isInvis = false;
    }

    private IEnumerator Blinking()
    {
        while (isInvis)
        {
            player.enabled = false;
            yield return new WaitForSeconds(blinkRate);
            player.enabled = true;
            yield return new WaitForSeconds(blinkRate);
        }
    }
}