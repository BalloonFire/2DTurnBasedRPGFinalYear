using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatrolAi : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform leftBoundary;
    public Transform rightBoundary;
    public float moveSpeed = 2f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public bool stopWhileAttacking = true;

    private bool movingRight = true;
    private bool canAttack = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // Optional: If your boss has attack animations

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsPlayerInRange())
        {
            if (canAttack)
            {
                StartCoroutine(AttackRoutine());
            }

            if (stopWhileAttacking)
            {
                rb.velocity = Vector2.zero;
                return;
            }
        }

        Patrol();
    }

    private void Patrol()
    {
        if (movingRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            spriteRenderer.flipX = false;

            if (transform.position.x >= rightBoundary.position.x)
            {
                movingRight = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            spriteRenderer.flipX = true;

            if (transform.position.x <= leftBoundary.position.x)
            {
                movingRight = true;
            }
        }
    }

    private bool IsPlayerInRange()
    {
        if (PlayerOverworldController.Instance == null) return false;

        float distance = Vector2.Distance(transform.position, PlayerOverworldController.Instance.transform.position);
        return distance <= attackRange;
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        // Optional: Trigger attack animation
        if (animator) animator.SetTrigger("Attack");

        // Do damage here — example:
        PlayerOverworldController.Instance.TakeDamage(1, transform);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (leftBoundary && rightBoundary)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftBoundary.position, rightBoundary.position);
        }

        // Show attack range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
