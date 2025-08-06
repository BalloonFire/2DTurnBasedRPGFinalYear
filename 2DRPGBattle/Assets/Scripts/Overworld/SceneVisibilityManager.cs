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
        // Wait 1–2 frames to ensure scene load completes
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

        if (!isBattle && SceneTracker.returningFromBattle)
        {
            // Wait for everything to fully initialize (especially weapon/camera)
            yield return new WaitForSeconds(0.1f);

            if (player != null)
                player.SetActive(true);

            if (overworldCanvas != null)
                overworldCanvas.SetActive(true);

            Debug.Log("Returned from battle. Player + UI reactivated in scene: " + sceneName);

            SceneTracker.returningFromBattle = false;
        }
        else
        {
            if (player != null)
                player.SetActive(!isBattle);

            if (overworldCanvas != null)
                overworldCanvas.SetActive(!isBattle);

            Debug.Log("Scene '" + sceneName + "' loaded. Player and UI active: " + !isBattle);
        }
    }
}
