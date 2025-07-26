using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            Debug.LogError("No players loaded from overworld!");
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
