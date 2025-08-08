using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // add this
using Player.Model;
using Player;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    void Awake()
    {
        var playersFromOverworld = GameDataCarrier.Instance?.GetSelectedPlayers();

        if (playersFromOverworld == null || playersFromOverworld.Count == 0)
        {
            // Only log error if NOT in battle scene
            if (SceneManager.GetActiveScene().name != "BattleTest" && SceneManager.GetActiveScene().name != "BattleBoss" && SceneManager.GetActiveScene().name != "Menu Scenes")
            {
                Debug.LogError("No players loaded from overworld!");
            }
            return;
        }

        for (int i = 0; i < playersFromOverworld.Count; i++)
        {
            if (i >= spawnPoints.Length) break;

            PlayerSO clonedSO = Instantiate(playersFromOverworld[i]);

            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);
            var controller = player.GetComponent<PlayerController>();
            controller.playerData = clonedSO;
            controller.playerID = (PlayerID)i;
        }
    }
}
