using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneVisibilityManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject overworldCanvas;
    [SerializeField] private string[] battleScenes;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(HandleVisibilityWithDelay(scene.name));
    }

    private IEnumerator HandleVisibilityWithDelay(string sceneName)
    {
        // Delay to ensure everything is loaded
        yield return null;
        yield return null;

        bool isBattle = false;

        foreach (string battleScene in battleScenes)
        {
            if (sceneName == battleScene)
            {
                isBattle = true;
                break;
            }
        }

        if (player != null)
            player.SetActive(!isBattle);

        if (overworldCanvas != null)
            overworldCanvas.SetActive(!isBattle);

        Debug.Log("Scene '" + sceneName + "' loaded. Player and UI active: " + !isBattle);
    }
}
