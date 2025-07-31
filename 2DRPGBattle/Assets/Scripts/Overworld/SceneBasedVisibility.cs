using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBasedVisibility : MonoBehaviour
{
    [SerializeField] private string[] scenesToHideIn;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        HandleScene(SceneManager.GetActiveScene().name);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleScene(scene.name);
    }

    private void HandleScene(string sceneName)
    {
        foreach (var name in scenesToHideIn)
        {
            if (sceneName == name)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        // If not a hidden scene, make sure it's active
        gameObject.SetActive(true);
    }
}
