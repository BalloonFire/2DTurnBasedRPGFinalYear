using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public int health;
    private int maxHealth;
    public Image healthBar;
    public GameObject enemyBar;
    public int minDmg;
    public int maxDmg;
    public int critChance;

    public int minDmg2;
    public int maxDmg2;
    public int critChance2;

    private PlayerController playerController;
    private Animator ani;

    private bool useAttack1 = true;
    private bool isAttacking;

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
            getHit(100);
        }
    }

    // Method to alternate between attacks
    public void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            isAttacking = false;
            return;
        }

        if (useAttack1)
        {
            StartCoroutine(SlideAndAttack(player, "attack1", minDmg, maxDmg, critChance));
        }
        else
        {
            StartCoroutine(SlideAndAttack(player, "attack2", minDmg2, maxDmg2, critChance2));
        }

        useAttack1 = !useAttack1;
    }

    IEnumerator SlideAndAttack(GameObject player, string attackTrigger, int minDamage, int maxDamage, int critChance)
    {
        enemyBar.SetActive(false);

        Vector3 startPosition = transform.position;
        Vector3 playerPosition = player.transform.position;
        Vector3 attackPosition = new Vector3(playerPosition.x + 1.0f, startPosition.y, startPosition.z);

        yield return StartCoroutine(SlideToPosition(attackPosition, 0.2f));

        ani.SetTrigger(attackTrigger);
        yield return new WaitForSeconds(0.7f);

        int crit = Random.Range(0, 100);
        if (crit <= critChance)
        {
            minDamage = Mathf.RoundToInt(minDamage * 1.5f);
            maxDamage = Mathf.RoundToInt(maxDamage * 1.5f);
            Debug.Log("Enemy Crit!");
        }

        int damage = Random.Range(minDamage, maxDamage);
        Debug.Log($"Enemy {attackTrigger} Damage: {damage}");
        playerController.getHit(damage);

        yield return StartCoroutine(SlideToPosition(startPosition, 0.2f));
        isAttacking = false;
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

        transform.position = target;
    }

    public void getHit(int dmgTaken)
    {
        enemyBar.SetActive(true);
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
            enemyBar.SetActive(false);
            ani.SetBool("isDead", true); // Trigger death animation
            ani.SetTrigger("hurt");
            Debug.Log("NPC is dead!");
            return;
        } else {
            ani.SetTrigger("hurt");
        }
    }
}
