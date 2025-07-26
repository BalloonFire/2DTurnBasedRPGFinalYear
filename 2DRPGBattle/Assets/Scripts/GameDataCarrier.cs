using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Model;

public class GameDataCarrier : MonoBehaviour
{
    public static GameDataCarrier Instance;

    [Header("Battle Team Setup")]
    public List<PlayerSO> selectedPlayers = new List<PlayerSO>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    public void SetSelectedPlayers(List<PlayerSO> players)
    {
        selectedPlayers = players;
    }

    public List<PlayerSO> GetSelectedPlayers()
    {
        return selectedPlayers;
    }

    public void Clear()
    {
        selectedPlayers.Clear();
    }
}
