using UnityEngine;

public class SceneTracker : MonoBehaviour
{
    public static SceneTracker Instance;

    public string previousSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPreviousScene(string name)
    {
        previousSceneName = name;
    }

    public string GetPreviousScene()
    {
        return previousSceneName;
    }
}
