using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Player identification (new addition)
    public enum PlayerID { Player1, Player2, Player3 }
    public PlayerID playerID;

    private int attackSelected = -1;

    public int health;
    private int maxHealth;
    public Image healthBar;

    public int minDmgAtk;
    public int maxDmgAtk;
    public int critChanceAtk;

    public int minDmgSkill;
    public int maxDmgSkill;
    public int critChanceSkill;

    public int minDmgUltimate;
    public int maxDmgUltimate;
    public int critChanceUltimate;

    private Animator ani;

    public Button attackButton;
    public Button skillButton;
    public Button ultimateButton;

    private const int maxMana = 5;
    public int currentMana;

    public TextMeshProUGUI manaText;
    private bool isAttacking;

    // Reference to battle handler (new addition)
    private BattleHandler battleHandler;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        ani = GetComponent<Animator>();
        battleHandler = FindObjectOfType<BattleHandler>();

        // Initialize based on player ID (modified)
        switch (playerID)
        {
            case PlayerID.Player1:
                if (attackButton != null)
                    attackButton.onClick.AddListener(() => SelectAttack(0));
                if (skillButton != null)
                    skillButton.onClick.AddListener(() => SelectAttack(1));
                if (ultimateButton != null)
                    ultimateButton.onClick.AddListener(() => SelectAttack(2));
                break;

            case PlayerID.Player2:
                // Player 2 specific initialization
                break;

            case PlayerID.Player3:
                // Player 3 specific initialization
                break;
        }

        Debug.Log(playerID + " initialized. Current attackSelected: " + attackSelected);
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaUI();
        Debug.Log(playerID + $" Mana Updated: {manaText.text}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            //getHit(100);
        }
    }

    public void SelectAttack(int attackId)
    {
        attackSelected = attackId;
        Debug.Log(playerID + " selected: " +
            (attackId == 0 ? "Basic Attack" :
             attackId == 1 ? "Skill Attack" : "Ultimate Attack"));

        // Notify battle handler (new addition)
        if (battleHandler != null)
        {
            battleHandler.PlayerReady(this);
        }
    }

    void UpdateManaUI()
    {
        if (manaText != null)
            manaText.text = playerID + " Mana: " + currentMana + "/" + maxMana;

        // Disable skill button if not enough mana
        if (skillButton != null)
            skillButton.interactable = currentMana > 0;

        // Disable ultimate button unless mana is full
        if (ultimateButton != null)
            ultimateButton.interactable = currentMana == maxMana;
    }

    public void Attack(GameObject enemy)
    {
        if (enemy == null || isAttacking) return; // Prevent multiple attacks

        isAttacking = true; // Lock attack to prevent duplicates

        Debug.Log(playerID + " current attackSelected: " + attackSelected);

        int crit = Random.Range(0, 100);
        int minDamage = 0, maxDamage = 0, critChance = 0;
        string attackType = "";

        // Determine attack type and values
        if (attackSelected == 0) // Basic Attack
        {
            attackType = "Basic Attack";
            minDamage = minDmgAtk;
            maxDamage = maxDmgAtk;
            critChance = critChanceAtk;
            currentMana = Mathf.Min(currentMana + 1, maxMana);
            UpdateManaUI();
        }
        else if (attackSelected == 1) // Skill Attack
        {
            if (currentMana > 0)
            {
                attackType = "Skill Attack";
                minDamage = minDmgSkill;
                maxDamage = maxDmgSkill;
                critChance = critChanceSkill;
                currentMana = Mathf.Max(currentMana - 1, 0);
                UpdateManaUI();
            }
            else
            {
                Debug.Log(playerID + " Not enough mana!");
                isAttacking = false; // Unlock attack
                return;
            }
        }
        else if (attackSelected == 2) // Ultimate Attack
        {
            if (currentMana == maxMana)
            {
                attackType = "Ultimate Attack";
                minDamage = minDmgUltimate;
                maxDamage = maxDmgUltimate;
                critChance = critChanceUltimate;
                currentMana = 0;
                UpdateManaUI();
            }
            else
            {
                Debug.Log(playerID + " Not enough mana for Ultimate!");
                isAttacking = false; // Unlock attack
                return;
            }
        }
        else
        {
            Debug.Log(playerID + " No attack initialized");
            isAttacking = false; // Unlock attack
            return;
        }

        StartCoroutine(SlideAndAttack(enemy, attackType, minDamage, maxDamage, critChance, crit));
    }

    IEnumerator SlideAndAttack(GameObject enemy, string attackType, int minDamage, int maxDamage, int critChance, int crit)
    {
        Vector3 startPosition = transform.position;
        Vector3 enemyPosition = enemy.transform.position;

        // Ensure the player moves only on the X-axis (prevent Y movement)
        Vector3 attackPosition = new Vector3(enemyPosition.x - 1.0f, startPosition.y, startPosition.z);

        // Move towards enemy
        yield return StartCoroutine(SlideToPosition(attackPosition, 0.2f));

        // Play attack animation only once
        ani.SetTrigger("attack");

        // Wait for the animation duration (adjust if needed)
        yield return new WaitForSeconds(0.7f);

        // Apply critical hit logic
        if (crit <= critChance)
        {
            Debug.Log(playerID + " Crit!");
            minDamage = Mathf.RoundToInt(minDamage * 1.5f);
            maxDamage = Mathf.RoundToInt(maxDamage * 1.5f);
        }

        // Calculate and apply damage
        int damage = Random.Range(minDamage, maxDamage);
        Debug.Log(playerID + $" Total {attackType}: {damage}");
        enemy.GetComponent<EnemyController>().getHit(damage);

        // Move back to start position
        yield return StartCoroutine(SlideToPosition(startPosition, 0.2f));

        isAttacking = false; // Unlock attack after completion

        // Notify battle handler (new addition)
        if (battleHandler != null)
        {
            battleHandler.PlayerAttackComplete(this);
        }
    }

    IEnumerator SlideToPosition(Vector3 target, float duration)
    {
        float elapsed = 0;
        Vector3 startingPos = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startingPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Ensure exact positioning
    }

    public void getHit(int dmgTaken)
    {
        health -= dmgTaken;
        if (health < 0) health = 0;
        // Calculate health percentage based on maxHealth
        float healthPercentage = (float)health / maxHealth;
        float newWidth = 300f * healthPercentage;
        // Debugging
        Debug.Log(playerID + $" Health: {health}/{maxHealth}, Health %: {healthPercentage * 100}%, New Width: {newWidth}");
        healthBar.rectTransform.sizeDelta = new Vector2(newWidth, healthBar.rectTransform.sizeDelta.y);
        // Set animation to play based on states
        if (health <= 0)
        {
            ani.SetBool("isDead", true); // Trigger death animation
            ani.SetTrigger("hurt");
            Debug.Log(playerID + " Player is dead!");
            if (battleHandler != null)
            {
                battleHandler.CheckGameOver(); // Check for gameover
            }
            return;
        }
        else
        {
            ani.SetTrigger("hurt");
        }
    }

    // New helper method
    public bool IsAlive()
    {
        return health > 0;
    }
}