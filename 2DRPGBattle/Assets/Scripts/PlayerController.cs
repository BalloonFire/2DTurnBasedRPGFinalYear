using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private int attackSelected = -1;

    public int health;
    private int maxHealth;
    public Image healthBar;
    public GameObject playerBar;
    public int minDmgAtk;
    public int maxDmgAtk;
    public int critChanceAtk;

    public int minDmgSkill;
    public int maxDmgSkill;
    public int critChanceSkill;

    private Animator ani;

    public Button attackButton;
    public Button skillButton;

    private const int maxMana = 5;
    public int currentMana;

    public TextMeshProUGUI manaText;
    private bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        maxHealth = health;
        ani = GetComponent<Animator>();

        // Assign button events (if buttons are set in Inspector)
        if (attackButton != null)
            attackButton.onClick.AddListener(() => SelectAttack(0)); // Basic Attack

        if (skillButton != null)
            skillButton.onClick.AddListener(() => SelectAttack(1)); // Skill Attack

        Debug.Log("Current attackSelected: " + attackSelected);
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaUI();
        Debug.Log($"Mana Updated: {manaText.text}");
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
        Debug.Log("Attack Selected: " + (attackId == 0 ? "Basic Attack" : "Skill Attack"));

    }

    void UpdateManaUI()
    {
        if (manaText != null)
            manaText.text = "Mana: " + currentMana + "/" + maxMana;

        // Disable skill button if not enough mana
        if (skillButton != null)
            skillButton.interactable = currentMana > 0;
    }

    public void Attack(GameObject enemy)
    {
        if (enemy == null || isAttacking) return; // Prevent multiple attacks

        isAttacking = true; // Lock attack to prevent duplicates

        Debug.Log("Current attackSelected: " + attackSelected);

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
                Debug.Log("Not enough mana!");
                isAttacking = false; // Unlock attack
                return;
            }
        }
        else
        {
            Debug.Log("No attack initialized");
            isAttacking = false; // Unlock attack
            return;
        }

        StartCoroutine(SlideAndAttack(enemy, attackType, minDamage, maxDamage, critChance, crit));
    }

    IEnumerator SlideAndAttack(GameObject enemy, string attackType, int minDamage, int maxDamage, int critChance, int crit)
    {
        playerBar.SetActive(false);

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
            Debug.Log("Crit!");
            minDamage = Mathf.RoundToInt(minDamage * 1.5f);
            maxDamage = Mathf.RoundToInt(maxDamage * 1.5f);
        }

        // Calculate and apply damage
        int damage = Random.Range(minDamage, maxDamage);
        Debug.Log($"Total {attackType}: {damage}");
        enemy.GetComponent<EnemyController>().getHit(damage);

        // Move back to start position
        yield return StartCoroutine(SlideToPosition(startPosition, 0.2f));

        isAttacking = false; // Unlock attack after completion
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
        playerBar.SetActive(true);
        health -= dmgTaken;
        if (health < 0) health = 0;
        // Calculate health percentage based on maxHealth
        float healthPercentage = (float)health / maxHealth;
        float newWidth = 300f * healthPercentage;
        // Debugging
        Debug.Log($"Health: {health}/{maxHealth}, Health %: {healthPercentage * 100}%, New Width: {newWidth}");
        healthBar.rectTransform.sizeDelta = new Vector2(newWidth, healthBar.rectTransform.sizeDelta.y);
        // Set animation to play based on states
        if (health <= 0)
        {
            playerBar.SetActive(false);
            ani.SetBool("isDead", true); // Trigger death animation
            ani.SetTrigger("hurt");
            Debug.Log("Player is dead!");
            FindObjectOfType<BattleHandler>().CheckGameOver(); // Check for gameover
            return;
        } else {
            ani.SetTrigger("hurt");
        }
    }
}
