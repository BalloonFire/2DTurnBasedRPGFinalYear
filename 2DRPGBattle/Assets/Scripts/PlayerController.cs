using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player.Model;
using System.Collections;
using Enemy;

namespace Player
{
    public enum PlayerID { Player1, Player2, Player3 }

    public class PlayerController : MonoBehaviour
    {
        [Header("Class Data")]
        public PlayerSO playerData;

        [Header("Player Identification")]
        public PlayerID playerID;

        [Header("UI References")]
        public Image healthBar;
        public Button attackButton;
        public Button skillButton;
        public Button ultimateButton;
        public TextMeshProUGUI manaText;

        // Runtime stats
        private int health;
        private int maxHealth;
        private const int maxMana = 5;
        private int currentMana = 0;

        // Combat state
        private Animator animator;
        private BattleHandler battleHandler;
        private bool isAttacking;
        [HideInInspector] public int attackSelected = -1;
        [HideInInspector] public bool canBeClicked = false;
        private Vector3 originalPosition;

        void Start()
        {
            if (playerData == null)
            {
                Debug.LogError("PlayerData SO not assigned!", this);
                return;
            }

            InitializeFromData();
            SetupComponents();
            SetupUIListeners();
        }

        private void InitializeFromData()
        {
            maxHealth = playerData.baseHealth;
            health = maxHealth;

            animator = GetComponent<Animator>();
            if (playerData.animatorController != null)
            {
                animator.runtimeAnimatorController = playerData.animatorController;
            }
        }

        private void SetupComponents()
        {
            battleHandler = FindObjectOfType<BattleHandler>();
            originalPosition = transform.position;
        }

        private void SetupUIListeners()
        {
            attackButton.onClick.AddListener(() => SelectAttack(0));
            skillButton.onClick.AddListener(() => SelectAttack(1));
            ultimateButton.onClick.AddListener(() => SelectAttack(2));

            UpdateManaUI();
            UpdateAttackButtons();
        }

        public void SelectAttack(int attackType)
        {
            if (isAttacking) return;
            attackSelected = attackType;
            battleHandler?.PlayerReady(this);
        }

        public void UpdateAttackButtons()
        {
            if (skillButton != null)
                skillButton.interactable = currentMana >= playerData.skillManaCost;

            if (ultimateButton != null)
                ultimateButton.interactable = currentMana >= playerData.ultimateManaCost;

            UpdateManaUI();
        }

        public void Attack(GameObject target)
        {
            if (isAttacking || target == null || attackSelected == -1) return;
            StartCoroutine(ExecuteAttack(target));
        }

        public void SetInteractable(bool canSelect)
        {
            GetComponent<Collider2D>().enabled = canSelect;
            canBeClicked = canSelect;

            // Visual feedback
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color = canSelect ? Color.white : Color.gray;
        }

        private IEnumerator ExecuteAttack(GameObject target)
        {
            isAttacking = true;
            EnemyController enemy = target.GetComponent<EnemyController>();

            int minDamage, maxDamage, critChance;
            string attackTrigger;

            switch (attackSelected)
            {
                case 1: // Skill
                    minDamage = playerData.minDmgSkill;
                    maxDamage = playerData.maxDmgSkill;
                    critChance = playerData.critChanceSkill;
                    currentMana -= playerData.skillManaCost;
                    attackTrigger = "skill";
                    break;
                case 2: // Ultimate
                    minDamage = playerData.minDmgUltimate;
                    maxDamage = playerData.maxDmgUltimate;
                    critChance = playerData.critChanceUltimate;
                    currentMana -= playerData.ultimateManaCost;
                    attackTrigger = "ultimate";
                    break;
                default: // Basic
                    minDamage = playerData.minDmgAtk;
                    maxDamage = playerData.maxDmgAtk;
                    critChance = playerData.critChanceAtk;
                    currentMana = Mathf.Min(currentMana + 1, maxMana);
                    attackTrigger = "attack";
                    break;
            }

            UpdateManaUI();
            UpdateAttackButtons();

            Vector3 attackPos = new Vector3(target.transform.position.x - 1.5f, transform.position.y, transform.position.z);
            yield return StartCoroutine(SlideToPosition(attackPos, 0.2f));

            animator.SetTrigger(attackTrigger);
            yield return new WaitForSeconds(0.5f);

            bool isCrit = Random.Range(0, 100) < critChance;
            int damage = isCrit ? Mathf.RoundToInt(Random.Range(minDamage, maxDamage) * 1.5f) :
                                Random.Range(minDamage, maxDamage);

            if (enemy != null)
                enemy.getHit(damage);

            yield return StartCoroutine(SlideToPosition(originalPosition, 0.2f));

            isAttacking = false;
            attackSelected = -1;
            battleHandler?.NotifyPlayerFinished(this);
        }

        public void getHit(int damage)
        {
            health = Mathf.Max(health - damage, 0);
            UpdateHealthUI();

            if (health <= 0)
            {
                animator.SetBool("isDead", true);
                battleHandler?.CheckGameOver();
            }
            else
            {
                animator.SetTrigger("hurt");
            }
        }

        private void UpdateHealthUI()
        {
            if (healthBar != null)
            {
                healthBar.rectTransform.sizeDelta = new Vector2(
                    300 * ((float)health / maxHealth),
                    healthBar.rectTransform.sizeDelta.y
                );
            }
        }

        private void UpdateManaUI()
        {
            if (manaText != null)
            {
                manaText.text = $"{playerID} Mana: {currentMana}/{maxMana}";
            }
        }

        public bool IsAlive() => health > 0;
        public bool IsAttacking() => isAttacking;

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

        void OnDestroy()
        {
            // Clean up event listeners
            attackButton.onClick.RemoveAllListeners();
            skillButton.onClick.RemoveAllListeners();
            ultimateButton.onClick.RemoveAllListeners();
        }
    }
}