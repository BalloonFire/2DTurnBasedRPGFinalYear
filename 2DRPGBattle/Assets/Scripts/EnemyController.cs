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

    private PlayerController playerController;
    private Animator ani;

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
        if (Input.GetButtonUp("Fire1"))
        {
            //Attack1();
        }
    }

    public void Attack1()
    {
        ani.SetTrigger("attack1");
        int crit = Random.Range(0, 100);
        if (crit <= critChance)
        {
            Debug.Log("Crit");
            float min = minDmg * 1.5f;
            float max = maxDmg * 1.5f;
            minDmg = Mathf.RoundToInt(min);
            maxDmg = Mathf.RoundToInt(max);
        }
        int dmg = Random.Range(minDmg, maxDmg);
        Debug.Log(dmg);

        playerController.getHit(dmg);
    }

    public void Attack2()
    {
        ani.SetTrigger("attack2");
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
