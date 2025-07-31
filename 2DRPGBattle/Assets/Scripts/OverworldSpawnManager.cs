using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldSpawnManager : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        if (SceneTracker.Instance != null && SceneTracker.Instance.GetPreviousScene() == "BattleScene")
        {
            Vector3 spawnPos = SceneTracker.Instance.playerReturnPosition;
            Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        }
    }
}
