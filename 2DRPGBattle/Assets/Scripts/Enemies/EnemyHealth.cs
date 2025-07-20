using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockedBackThrust = 15f;

    private int currentHealth;
    private Knockback knockback;
    private Flash flash;

    private void Awake()
    {   
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        knockback.GetKnockedBack(PlayerController1.Instance.transform, knockedBackThrust);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckedDetectDeathRoutine());
    }

    private IEnumerator CheckedDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            // Optional: play death VFX now or after battle
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

            // Save ID before battle
            string slimeID = GetComponent<EnemyTrigger>()?.enemyID;
            if (!string.IsNullOrEmpty(slimeID))
            {
                PlayerPrefs.SetString("BattleEnemyID", slimeID);
                PlayerPrefs.SetString("ReturnScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                PlayerPrefs.SetFloat("PlayerX", PlayerController1.Instance.transform.position.x);
                PlayerPrefs.SetFloat("PlayerY", PlayerController1.Instance.transform.position.y);
            }

            // Load the battle scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("BattleTest");
        }
    }
}

