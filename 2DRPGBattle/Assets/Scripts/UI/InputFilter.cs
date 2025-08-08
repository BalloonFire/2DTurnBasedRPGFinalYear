using UnityEngine;
using UnityEngine.EventSystems;

public class InputFilter : MonoBehaviour
{
    public static InputFilter Instance { get; private set; }

    private bool inputProcessedThisFrame = false;

    private void Awake()
    {
        // Singleton pattern without DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Reset input flag at the start of each frame
        inputProcessedThisFrame = false;
    }

    public bool GetActionInput()
    {
        if (inputProcessedThisFrame) return false;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            inputProcessedThisFrame = true;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputProcessedThisFrame = true;
            return true;
        }

        return false;
    }

    public float GetHorizontalInput()
    {
        if (inputProcessedThisFrame) return 0f;

        float input = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(input) > 0.1f)
        {
            inputProcessedThisFrame = true;
            return input;
        }

        return 0f;
    }
}
