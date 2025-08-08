using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Player.Model;

public class PlayerOverworldController : Singleton<PlayerOverworldController>
{
    [Header("References")]
    [SerializeField] private PlayerSO playerData;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashMultiplier = 4f;
    [SerializeField] private InputActionReference joystick;     // Gamepad movement
    [SerializeField] private InputActionReference dashButton;   // Gamepad dash

    [Header("Health Settings")]
    [SerializeField] private string healthSliderName = "Health Slider";
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private PlayerControls playerControls;
    public PlayerControls Controls => playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Knockback knockback;
    private Flash flash;
    private Slider healthSlider;

    private bool facingLeft = false;
    private bool isDashing = false;
    private bool canTakeDamage = true;

    private float baseMoveSpeed;
    private int maxHealth;
    private int currentHealth;

    public bool FacingLeft => facingLeft;
    public Transform GetWeaponCollider() => weaponCollider;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
        flash = GetComponent<Flash>();
    }

    private void Start()
    {
        if (playerData != null)
        {
            maxHealth = playerData.baseHealth;
            animator.runtimeAnimatorController = playerData.overworldAnimatorController;
        }
        else
        {
            Debug.LogWarning("PlayerSO not assigned!");
            maxHealth = 100;
        }

        currentHealth = maxHealth;
        baseMoveSpeed = moveSpeed;

        if (dashButton != null)
        {
            dashButton.action.performed += _ => Dash();
            dashButton.action.Enable();
        }

        if (joystick != null)
            joystick.action.Enable();

        UpdateHealthSlider();
    }

    private void OnDisable()
    {
        // Disable PlayerControls action maps
        if (playerControls != null)
            playerControls.Disable();

        // Also disable joystick and dashButton InputActionReferences to avoid leak
        if (dashButton != null)
            dashButton.action.Disable();

        if (joystick != null)
            joystick.action.Disable();
    }

    private void OnEnable()
    {
        if (playerControls != null)
            playerControls.Enable();

        if (dashButton != null)
            dashButton.action.Enable();

        if (joystick != null)
            joystick.action.Enable();

        if (CameraController.Instance != null)
        {
            CameraController.Instance.SetPlayerCameraFollow();
        }

        if (weaponCollider != null)
        {
            weaponCollider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        AdjustFacingDirection();
        Move();
    }

    private void HandleInput()
    {
        Vector2 keyboardMovement = playerControls.Movement.Move.ReadValue<Vector2>();
        Vector2 gamepadMovement = joystick != null ? joystick.action.ReadValue<Vector2>() : Vector2.zero;

        // Combine both input sources
        movement = keyboardMovement + gamepadMovement;

        if (movement.magnitude > 1f)
            movement = movement.normalized;

        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        if (knockback.GettingKnockedBack) return;

        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        facingLeft = mousePos.x < playerScreenPos.x;
        spriteRenderer.flipX = facingLeft;
    }

    private void Dash()
    {
        if (!isDashing && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();
            isDashing = true;
            moveSpeed *= dashMultiplier;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = 0.2f;
        float dashCooldown = 0.25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = baseMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
    }

    public void HealPlayer(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            UpdateHealthSlider();
        }
    }
    public void PrepareForBattle()
    {
        BattleTransition.SetPlayerData(playerData);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyAI enemy))
        {
            TakeDamage(20, collision.transform);
        }
    }

    public void TakeDamage(int damage, Transform hitSource)
    {
        if (!canTakeDamage) return;

        knockback.GetKnockedBack(hitSource, knockBackThrustAmount);
        flash.StartFlash();
        currentHealth -= damage;
        canTakeDamage = false;
        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfDead();
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player has died!");
            // Add death handling logic here
        }
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider == null)
        {
            GameObject sliderObj = GameObject.Find(healthSliderName);
            if (sliderObj) healthSlider = sliderObj.GetComponent<Slider>();
        }

        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}
