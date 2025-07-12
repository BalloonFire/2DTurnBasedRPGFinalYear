using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Enemy.Model;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Class Data")]
        public EnemySO enemyData;
        public enum EnemyID { Enemy1, Enemy2, Enemy3 }
        public EnemyID enemyID;

        [Header("UI Elements")]
        public Image healthBar;
        public GameObject enemyBar;

        private int health;
        private int maxHealth;
        private Animator ani;
        private bool useAttack1 = true;
        private bool isAttacking;

        private void Awake()
        {
            // Initialize with the assigned enemyData if it exists
            if (enemyData != null)
            {
                Initialize(enemyData);
            }
        }

        public void Initialize(EnemySO data)
        {
            enemyData = data;

            // Set health stats
            maxHealth = enemyData.baseHealth;
            health = maxHealth;

            // Get components
            ani = GetComponent<Animator>();

            // Set animator controller if provided
            if (enemyData.animatorController != null)
            {
                ani.runtimeAnimatorController = enemyData.animatorController;
            }

            // Initialize UI
            if (healthBar != null)
            {
                UpdateHealthBar();
            }

            enemyBar.SetActive(false);
        }

        public IEnumerator ExecuteAttack(PlayerController[] players)
        {
            if (isAttacking || health <= 0) yield break;
            isAttacking = true;

            enemyBar.SetActive(true);

            var alivePlayers = System.Array.FindAll(players, p => p.IsAlive());
            if (alivePlayers.Length == 0)
            {
                isAttacking = false;
                yield break;
            }

            GameObject randomPlayer = alivePlayers[Random.Range(0, alivePlayers.Length)].gameObject;

            if (useAttack1)
            {
                yield return StartCoroutine(PerformAttack(
                    randomPlayer,
                    "attack1",
                    enemyData.minDmg,
                    enemyData.maxDmg,
                    enemyData.critChance
                ));
            }
            else
            {
                yield return StartCoroutine(PerformAttack(
                    randomPlayer,
                    "attack2",
                    enemyData.minDmg2,
                    enemyData.maxDmg2,
                    enemyData.critChance2
                ));
            }

            useAttack1 = !useAttack1;
            isAttacking = false;
            enemyBar.SetActive(false);
        }

        private IEnumerator PerformAttack(GameObject player, string trigger, int minDmg, int maxDmg, int critChance)
        {
            Vector3 startPos = transform.position;
            Vector3 attackPos = new Vector3(player.transform.position.x + 1f, startPos.y, startPos.z);

            yield return StartCoroutine(SlideToPosition(attackPos, 0.2f));

            ani.SetTrigger(trigger);
            yield return new WaitForSeconds(0.7f);

            bool isCrit = Random.Range(0, 100) <= critChance;
            int damage = CalculateDamage(minDmg, maxDmg, isCrit);

            Debug.Log($"{enemyID} deals {damage} damage{(isCrit ? " (CRIT!)" : "")} to {player.GetComponent<PlayerController>().playerID}");
            player.GetComponent<PlayerController>().getHit(damage);

            yield return StartCoroutine(SlideToPosition(startPos, 0.2f));
        }

        private int CalculateDamage(int min, int max, bool isCrit)
        {
            int damage = Random.Range(min, max + 1);
            return isCrit ? Mathf.RoundToInt(damage * 1.5f) : damage;
        }

        private IEnumerator SlideToPosition(Vector3 target, float duration)
        {
            float elapsed = 0;
            Vector3 startPos = transform.position;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, target, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
        }

        public void getHit(int damage)
        {
            if (health <= 0) return;

            health = Mathf.Max(health - damage, 0);
            UpdateHealthBar();

            if (health <= 0)
            {
                HandleDeath();
            }
            else
            {
                ani.SetTrigger("hurt");
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBar == null) return;

            float healthPercent = (float)health / maxHealth;
            healthBar.rectTransform.sizeDelta = new Vector2(300f * healthPercent, healthBar.rectTransform.sizeDelta.y);
        }

        private void HandleDeath()
        {
            enemyBar.SetActive(false);
            ani.SetBool("isDead", true);
            ani.SetTrigger("hurt");
            FindObjectOfType<BattleHandler>().CheckGameOver();
        }

        public bool IsAlive()
        {
            return health > 0;
        }
    }
}