// TurnManager.cs - Manages player and enemy turns
using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public bool isPlayerTurn = true;
    public List<CharacterBase> partyMembers = new List<CharacterBase>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy's Turn!");
        yield return new WaitForSeconds(1f);
        
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            enemy.TakeTurn();
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("Player's Turn!");
        isPlayerTurn = true;
        
        foreach (CharacterBase character in partyMembers)
        {
            character.ResetAction();
        }
    }
}

// CharacterBase.cs - Base class for all player characters
public class CharacterBase : MonoBehaviour
{
    public string characterName;
    public bool hasActed = false;

    public virtual void BasicAttack() { }
    public virtual void UseSkill() { }
    public virtual void UseUltimate() { }
    public virtual void ResetAction() { hasActed = false; }
}

// KnightCharacter.cs - Implements Knight's abilities
public class KnightCharacter : CharacterBase
{
    public int basicAttackDamage = 20;
    public int skillDamage = 40;
    public int ultimateDamage = 75;

    public float skillManaCost = 30f;
    public float ultimateManaCost = 60f;

    public LayerMask enemyLayers;
    public Transform attackPoint;
    public float attackRange = 3f;

    private KnightMana mana;

    void Start()
    {
        mana = GetComponent<KnightMana>();
        TurnManager.Instance.partyMembers.Add(this);
    }

    void Update()
    {
        if (!TurnManager.Instance.isPlayerTurn || hasActed) return;

        if (Input.GetKeyDown(KeyCode.Q)) { BasicAttack(); }
        if (Input.GetKeyDown(KeyCode.E)) { UseSkill(); }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { UseUltimate(); }
    }

    public override void BasicAttack()
    {
        DealDamage(basicAttackDamage);
        hasActed = true;
        TurnManager.Instance.EndPlayerTurn();
    }

    public override void UseSkill()
    {
        if (mana.CanUseAbility(skillManaCost))
        {
            mana.UseMana(skillManaCost);
            DealDamage(skillDamage);
            hasActed = true;
            TurnManager.Instance.EndPlayerTurn();
        }
    }

    public override void UseUltimate()
    {
        if (mana.CanUseAbility(ultimateManaCost))
        {
            mana.UseMana(ultimateManaCost);
            DealDamage(ultimateDamage);
            hasActed = true;
            TurnManager.Instance.EndPlayerTurn();
        }
    }

    void DealDamage(int damage)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        }
    }
}

// KnightMana.cs - Handles mana usage
public class KnightMana : MonoBehaviour
{
    public float maxMana = 100f;
    private float currentMana;

    void Start() { currentMana = maxMana; }

    public bool CanUseAbility(float cost) { return currentMana >= cost; }
    public void UseMana(float cost) { currentMana -= cost; }
}

// EnemyAI.cs - Enemy turn logic and attack
public class EnemyAI : MonoBehaviour
{
    public int attackDamage = 20;
    public float attackRange = 3f;
    
    public void TakeTurn()
    {
        CharacterBase[] partyMembers = FindObjectsOfType<CharacterBase>();
        CharacterBase closest = null;
        float minDistance = Mathf.Infinity;

        foreach (CharacterBase member in partyMembers)
        {
            float dist = Vector3.Distance(transform.position, member.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = member;
            }
        }

        if (closest != null && minDistance <= attackRange)
        {
            AttackPlayer(closest);
        }
    }

    void AttackPlayer(CharacterBase target)
    {
        target.GetComponent<KnightHealth>()?.TakeDamage(attackDamage);
    }
}

// EnemyHealth.cs - Handles enemy health and death
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start() { currentHealth = maxHealth; }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) { Die(); }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}

// KnightHealth.cs - Handles knight's health
public class KnightHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start() { currentHealth = maxHealth; }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) { Die(); }
    }

    void Die()
    {
        Debug.Log("Knight has fallen!");
    }
}
