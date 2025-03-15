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
            //getHit(10);
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
    }

    public void Attack(GameObject enemy)
    {
        if (enemy == null) return;

        Debug.Log("Current attackSelected: " + attackSelected);

        int crit = Random.Range(0, 100);
        int minDamage = 0, maxDamage = 0, critChance = 0;
        string attackType = "";

        // Determine attack type and values
        if (attackSelected == 0) // Basic Attack
        {
            ani.SetTrigger("attack");
            attackType = "Basic Attack";
            minDamage = minDmgAtk;
            maxDamage = maxDmgAtk;
            critChance = critChanceAtk;
            currentMana = Mathf.Min(currentMana + 1, maxMana); // Regenerate mana on Basic Attack
            UpdateManaUI();
            Debug.Log($"Mana Updated: {manaText.text}");
        }
        else if (attackSelected == 1) // Skill Attack
        {
            if (currentMana > 0)
            {
                ani.SetTrigger("attack");
                attackType = "Skill Attack";
                minDamage = minDmgSkill;
                maxDamage = maxDmgSkill;
                critChance = critChanceSkill;
                currentMana = Mathf.Max(currentMana - 1, 0); // Use mana
                UpdateManaUI();
                Debug.Log($"Mana Updated: {manaText.text}");
            }
            else
            {
                Debug.Log("Not enough mana to use Skill Attack!");
                return; // Exit if no mana
            }
        }
        else
        {
            Debug.Log("No attack initialized");
            return;
        }

        // Apply critical hit logic
        if (crit <= critChance)
        {
            Debug.Log("Crit!");
            minDamage = Mathf.RoundToInt(minDamage * 1.5f);
            maxDamage = Mathf.RoundToInt(maxDamage * 1.5f);
        }

        // Calculate damage
        int damage = Random.Range(minDamage, maxDamage);
        Debug.Log($"Total {attackType}: {damage}");

        // Apply damage to enemy
        enemy.GetComponent<EnemyController>().getHit(damage);
    }

    public void getHit(int dmgTaken)
    {
        // Set animation to play based on states
        if (health <= 0)
        {
            health = 0;
            ani.SetBool("isDead", true); // Trigger death animation
            ani.SetTrigger("hurt");
            Debug.Log("Player is dead!");

            // Optionally disable player actions
            this.enabled = false;
            return;
        }
        ani.SetTrigger("hurt");
        health -= dmgTaken;
        if (health < 0) health = 0;
        // Calculate health percentage based on maxHealth
        float healthPercentage = (float)health / maxHealth;
        float newWidth = 300f * healthPercentage;
        // Debugging
        Debug.Log($"Health: {health}/{maxHealth}, Health %: {healthPercentage * 100}%, New Width: {newWidth}");
        healthBar.rectTransform.sizeDelta = new Vector2(newWidth, healthBar.rectTransform.sizeDelta.y);
    }
}
