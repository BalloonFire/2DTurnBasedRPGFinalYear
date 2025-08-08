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

        string enemyName = gameObject.name;
        if (enemyName.EndsWith("(Clone)"))
            enemyName = enemyName.Replace("(Clone)", "");

        if (DeadEnemiesManager.Instance != null &&
            DeadEnemiesManager.Instance.IsEnemyDefeated(enemyName))
        {
            Debug.Log($"Enemy {enemyName} is defeated. Destroying from overworld.");
            Destroy(gameObject);
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

        PlayerOverworldController.Instance.PrepareForBattle();

        // Apply knockback to enemy itself (your current code)
        if (PlayerOverworldController.Instance != null)
            knockback.GetKnockedBack(PlayerOverworldController.Instance.transform, knockedBackThrust);

        StartCoroutine(flash.FlashRoutine());

        hasTriggeredBattle = true;

        var player = PlayerOverworldController.Instance;
        if (player != null)
        {
            // Reset player's knockback immediately so it won't lock after battle
            var playerKnockback = player.GetComponent<Knockback>();
            if (playerKnockback != null)
            {
                playerKnockback.ResetKnockbackState();
            }

            // Reset flash state to avoid stuck flash
            var playerFlash = player.GetComponent<Flash>();
            if (playerFlash != null)
            {
                playerFlash.ResetFlash();
            }

            // Save the position so we can restore it after battle using SceneTracker
            SceneTracker.Instance.SetPlayerPosition(player.transform.position);
            SceneTracker.returningFromBattle = true;
        }

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
        StoreBattleInfo(player.transform);

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
            // Save the overworld enemy's name
            string enemyName = gameObject.name;
            if (enemyName.EndsWith("(Clone)"))
                enemyName = enemyName.Replace("(Clone)", "");

            BattleTransition.Instance.SetEnemy(enemyData, enemyName);

            // Save return scene
            if (SceneTracker.Instance != null)
            {
                SceneTracker.Instance.SetPreviousScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
