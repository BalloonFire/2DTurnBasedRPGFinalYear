using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneVisibilityManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject overworldCanvas;
    [SerializeField] private string[] battleScenes; // Include both normal and boss battle scenes

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (player != null && player.transform.parent == null) // Ensure root object
            DontDestroyOnLoad(player);

        if (overworldCanvas != null && overworldCanvas.transform.parent == null)
            DontDestroyOnLoad(overworldCanvas);

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

        // Special case for MenuScenes - disable everything
        if (sceneName == "MenuScenes")
        {
            if (player != null)
            {
                player.SetActive(false);
                DisableOverworldControls();
            }
            if (overworldCanvas != null)
                overworldCanvas.SetActive(false);

            // Play menu music
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMenuMusic();

            Debug.Log("Transitioned to MenuScenes - Player and UI disabled");
            yield break;
        }

        if (!isBattle && SceneTracker.returningFromBattle)
        {
            // Wait for everything to fully initialize (especially weapon/camera)
            yield return new WaitForSeconds(0.1f);

            if (player != null)
            {
                EnableOverworldControls();
                player.SetActive(true);
            }

            if (overworldCanvas != null)
                overworldCanvas.SetActive(true);

            Debug.Log("Returned from battle. Player + UI reactivated in scene: " + sceneName);

            // Play overworld music here
            PlayOverworldMusic(sceneName);

            SceneTracker.returningFromBattle = false;
        }
        else
        {
            if (player != null)
            {
                if (isBattle)
                {
                    DisableOverworldControls();
                }
                else
                {
                    EnableOverworldControls();
                }

                player.SetActive(!isBattle);
            }

            if (overworldCanvas != null)
                overworldCanvas.SetActive(!isBattle);

            Debug.Log("Scene '" + sceneName + "' loaded. Player and UI active: " + !isBattle);

            // Play battle or overworld music here
            if (isBattle)
            {
                if (AudioManager.Instance != null)
                {
                    if (sceneName == "BattleBoss")
                        AudioManager.Instance.PlayBossMusic();
                    else
                        AudioManager.Instance.PlayBattleMusic();
                }
            }
            else
            {
                PlayOverworldMusic(sceneName);
            }
        }
    }

    private void PlayOverworldMusic(string sceneName)
    {
        if (AudioManager.Instance == null) return;

        // Pick overworld track based on scene name
        switch (sceneName)
        {
            case "Map grass 1":
                AudioManager.Instance.PlayBGMusic1();
                break;
            case "Map grass 2":
                AudioManager.Instance.PlayBGMusic2();
                break;
            case "Dungeon 1":
                AudioManager.Instance.PlayBGMusic3();
                break;
            default:
                AudioManager.Instance.PlayMenuMusic();
                break;
        }
    }

    private void DisableOverworldControls()
    {
        var controller = player.GetComponent<PlayerOverworldController>();
        if (controller?.Controls != null)
        {
            controller.Controls.Movement.Disable();
            controller.Controls.Combat.Disable();
            controller.Controls.Inventory.Disable();
            Debug.Log("Overworld controls disabled for battle.");
        }
    }

    private void EnableOverworldControls()
    {
        var controller = player.GetComponent<PlayerOverworldController>();
        if (controller?.Controls != null)
        {
            controller.Controls.Movement.Enable();
            controller.Controls.Combat.Enable();
            controller.Controls.Inventory.Enable();
            Debug.Log("Overworld controls enabled.");
        }
    }
}
