using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFilter : MonoBehaviour
{
    public static InputFilter Instance { get; private set; }

    [Header("Button Display References")]
    public Image buttonSouthDisplay;  // A button
    public Image buttonEastDisplay;   // B button
    public Image leftStickDisplay;    // Left stick

    [Header("Button Sprites")]
    public Sprite buttonSouthSprite;  // A button sprite
    public Sprite buttonEastSprite;   // B button sprite
    public Sprite leftStickSprite;    // Left stick sprite
    public Sprite keyboardMouseSprite; // Fallback sprite

    private bool inputProcessedThisFrame = false;
    private bool usingGamepad = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateButtonDisplays();
    }

    private void Update()
    {
        // Reset input flag at the start of each frame
        inputProcessedThisFrame = false;

        // Detect input device change
        bool gamepadConnected = Input.GetJoystickNames().Length > 0;
        if (gamepadConnected != usingGamepad)
        {
            usingGamepad = gamepadConnected;
            UpdateButtonDisplays();
        }
    }

    private void UpdateButtonDisplays()
    {
        if (usingGamepad)
        {
            buttonSouthDisplay.sprite = buttonSouthSprite;
            buttonEastDisplay.sprite = buttonEastSprite;
            leftStickDisplay.sprite = leftStickSprite;
        }
        else
        {
            buttonSouthDisplay.sprite = keyboardMouseSprite;
            buttonEastDisplay.sprite = keyboardMouseSprite;
            leftStickDisplay.sprite = keyboardMouseSprite;
        }
    }

    public bool GetButtonSouthInput() // A button
    {
        if (inputProcessedThisFrame) return false;

        if (Input.GetButtonDown("Button South") ||
            (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()))
        {
            inputProcessedThisFrame = true;
            return true;
        }
        return false;
    }

    public bool GetButtonEastInput() // B button
    {
        if (inputProcessedThisFrame) return false;

        if (Input.GetButtonDown("Button East") || Input.GetKeyDown(KeyCode.Escape))
        {
            inputProcessedThisFrame = true;
            return true;
        }
        return false;
    }

    public Vector2 GetLeftStickInput()
    {
        if (inputProcessedThisFrame) return Vector2.zero;

        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        // Add keyboard fallback
        if (input.magnitude < 0.1f)
        {
            input = new Vector2(
                Input.GetAxisRaw("HorizontalKeyboard"),
                Input.GetAxisRaw("VerticalKeyboard")
            );
        }

        if (input.magnitude > 0.1f)
        {
            inputProcessedThisFrame = true;
            return input.normalized;
        }

        return Vector2.zero;
    }
}