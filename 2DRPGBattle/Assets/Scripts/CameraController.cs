using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Start()
    {
        SetPlayerCameraFollow();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow()
    {
        // Don't run in battle scenes
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Battle")) // or check with your battleScenes list
        {
            return;
        }

        cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineVirtualCamera == null)
        {
            Debug.LogWarning("No CinemachineVirtualCamera found in scene.");
            return;
        }

        if (PlayerOverworldController.Instance == null)
        {
            Debug.LogWarning("No PlayerOverworldController instance found in scene.");
            return;
        }

        cinemachineVirtualCamera.Follow = PlayerOverworldController.Instance.transform;
    }
}
