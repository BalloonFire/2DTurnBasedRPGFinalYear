using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldSpawnManager : MonoBehaviour
{
    private void Start()
    {
        // Check if SceneTracker has a stored position
        if (SceneTracker.Instance != null && SceneTracker.Instance.HasStoredPosition())
        {
            var player = PlayerOverworldController.Instance;

            if (player != null)
            {
                player.transform.position = SceneTracker.Instance.GetPlayerPosition();
                Debug.Log("Player repositioned to: " + player.transform.position);
            }
            else
            {
                Debug.LogWarning("PlayerOverworldController not found!");
            }
        }
    }
}
