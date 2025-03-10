using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int health;
    private int maxHealth;
    public Image healthBar;
    public int minDmgAtk;
    public int maxDmgAtk;
    public int critChanceAtk;

    public int minDmgSkill;
    public int maxDmgSkill;
    public int critChanceSkill;

    private int attackSelected = 0;

    private Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        maxHealth = health;
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            //getHit(10);
        }
    }
    public void Attack(GameObject enemy)
    {
        int crit = Random.Range(0, 100);
        if (attackSelected == 0)
        {
            //ani.SetTrigger("attack");
            if (crit <= critChanceAtk)
            {
                Debug.Log("Crit");
                float min = minDmgAtk * 1.5f;
                float max = maxDmgAtk * 1.5f;
                minDmgAtk = Mathf.RoundToInt(min);
                maxDmgAtk = Mathf.RoundToInt(max);
            }
            int dmg = Random.Range(minDmgAtk, maxDmgAtk);
            Debug.Log("Total Basic: " + dmg);

            enemy.GetComponent<EnemyController>().getHit(dmg);
        }
        else if (attackSelected == 1)
        {
            //ani.SetTrigger("skill");
            if (crit <= critChanceSkill)
            {
                Debug.Log("Crit");
                float min = minDmgSkill * 1.5f;
                float max = maxDmgSkill * 1.5f;
                minDmgSkill = Mathf.RoundToInt(min);
                maxDmgSkill = Mathf.RoundToInt(max);
            }
            int dmg = Random.Range(minDmgSkill, maxDmgSkill);
            Debug.Log("Total Skill: " + dmg);

            enemy.GetComponent<EnemyController>().getHit(dmg);
        }
        else
        {
            Debug.Log("No attack initialized");
        }
    }
    public void Skill(GameObject enemy)
    {
    }
    public void getHit(int dmgTaken)
    {
        // Set animation to play based on states
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
