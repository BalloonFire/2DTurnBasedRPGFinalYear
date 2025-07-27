using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Enemy.Model;

[RequireComponent(typeof(Flash), typeof(Knockback))]
public class EnemyOverworldController : MonoBehaviour
{
    public string battleScene = "BattleTest";

    [Header("Enemy Settings")]
    public EnemySO enemyData;

    [Header("VFX & Knockback")]
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockedBackThrust = 15f;

    private int currentHealth;
    private Knockback knockback;
    private Flash flash;
    private bool hasTriggeredBattle = false;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemySO not assigned on " + gameObject.name);
            return;
        }

        currentHealth = enemyData.baseHealth;

        // Apply animator from SO
        if (enemyData.overworldAnimatorController != null)
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null)
                anim.runtimeAnimatorController = enemyData.overworldAnimatorController;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with enemy — loading battle scene!");
            BattleTransition.Instance.SetEnemy(enemyData);
            StoreBattleInfo(other.transform);
            SceneManager.LoadScene(battleScene);
        }
    }

    public void TakeDamage(int damage)
    {
        if (hasTriggeredBattle) return; // prevent multiple triggers

        currentHealth -= damage;

        // Optional: knockback and flash
        if (PlayerOverworldController.Instance != null)
            knockback.GetKnockedBack(PlayerOverworldController.Instance.transform, knockedBackThrust);

        StartCoroutine(flash.FlashRoutine());

        // Now: trigger battle transition on first damage
        hasTriggeredBattle = true;
        StoreBattleInfo(PlayerOverworldController.Instance.transform);

        if (deathVFXPrefab != null)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        GameObject overworldUI = GameObject.Find("UIOverworldCanvas");
        if (overworldUI) overworldUI.SetActive(false);

        GameObject playerOverworld = GameObject.Find("Player");
        if (playerOverworld) playerOverworld.SetActive(false);

        SceneManager.LoadScene(battleScene);
    }

    private void StoreBattleInfo(Transform playerTransform)
    {
        if (enemyData != null)
        {
            BattleTransition.Instance.SetEnemy(enemyData);
            PlayerPrefs.SetString("ReturnScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetFloat("PlayerX", playerTransform.position.x);
            PlayerPrefs.SetFloat("PlayerY", playerTransform.position.y);
        }
    }
}
