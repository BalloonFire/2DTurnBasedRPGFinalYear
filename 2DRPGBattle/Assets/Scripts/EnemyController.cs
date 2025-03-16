using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public int health;
    private int maxHealth;
    public Image healthBar;
    public int minDmg;
    public int maxDmg;
    public int critChance;

    public int minDmg2;
    public int maxDmg2;
    public int critChance2;

    private PlayerController playerController;
    private Animator ani;

    private bool useAttack1 = true;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        maxHealth = health;
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            //getHit(100);
        }
    }

    // Method to alternate between attacks
    public void Attack()
    {
        if (useAttack1)
        {
            Attack1();
        }
        else
        {
            Attack2();
        }

        useAttack1 = !useAttack1; // Toggle attack flag
    }

    public void Attack1()
    {
        int crit = Random.Range(0, 100);
        int tempMinDmg = minDmg; // Store original damage values
        int tempMaxDmg = maxDmg;

        if (crit <= critChance)
        {
            Debug.Log("Crit!");
            tempMinDmg = Mathf.RoundToInt(tempMinDmg * 1.5f);
            tempMaxDmg = Mathf.RoundToInt(tempMaxDmg * 1.5f);
        }

        int dmg = Random.Range(tempMinDmg, tempMaxDmg);
        Debug.Log($"Enemy Attack 1 Damage: {dmg}");

        ani.SetTrigger("attack1");

        playerController.getHit(dmg);
    }

    public void Attack2()
    {
        int crit = Random.Range(0, 100);
        int tempMinDmg2 = minDmg2; // Store original damage values
        int tempMaxDmg2 = maxDmg2;

        if (crit <= critChance2)
        {
            Debug.Log("Crit!");
            tempMinDmg2 = Mathf.RoundToInt(tempMinDmg2 * 1.5f);
            tempMaxDmg2 = Mathf.RoundToInt(tempMaxDmg2 * 1.5f);
        }

        int dmg = Random.Range(tempMinDmg2, tempMaxDmg2);
        Debug.Log($"Enemy Attack 2 Damage: {dmg}");

        ani.SetTrigger("attack2");

        playerController.getHit(dmg);
    }

    public void getHit(int dmgTaken)
    {
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
            ani.SetBool("isDead", true); // Trigger death animation
            ani.SetTrigger("hurt");
            Debug.Log("NPC is dead!");
            return;
        } else {
            ani.SetTrigger("hurt");
        }
    }
}
