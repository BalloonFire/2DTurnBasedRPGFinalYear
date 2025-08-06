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

    public void TakeDamage(int damage)
    {
        if (hasTriggeredBattle) return;

        currentHealth -= damage;

        // Apply knockback
        if (PlayerOverworldController.Instance != null)
            knockback.GetKnockedBack(PlayerOverworldController.Instance.transform, knockedBackThrust);

        StartCoroutine(flash.FlashRoutine());

        hasTriggeredBattle = true;

        // Disable weapon collider
        if (PlayerOverworldController.Instance != null)
        {
            Transform weapon = PlayerOverworldController.Instance.GetWeaponCollider();
            if (weapon != null)
                weapon.gameObject.SetActive(false);
        }

        if (ActiveWeapon.Instance != null)
        {
            ActiveWeapon.Instance.ResetAttackState();
        }

        // Save scene and set enemy
        StoreBattleInfo(PlayerOverworldController.Instance.transform);
        BattleTransition.Instance.SetEnemy(enemyData);

        // Optional VFX
        if (deathVFXPrefab != null)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        // Load battle scene
        SceneManager.LoadScene(battleScene);
    }

    private void StoreBattleInfo(Transform playerTransform)
    {
        if (enemyData != null)
        {
            // Save return scene name using SceneTracker
            if (SceneTracker.Instance != null)
            {
                SceneTracker.Instance.SetPreviousScene(SceneManager.GetActiveScene().name);
            }

            // Optionally keep position in PlayerPrefs
            PlayerPrefs.SetFloat("PlayerX", playerTransform.position.x);
            PlayerPrefs.SetFloat("PlayerY", playerTransform.position.y);
            PlayerPrefs.Save();
        }
    }
}
