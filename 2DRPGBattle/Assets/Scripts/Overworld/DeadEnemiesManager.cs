using System.Collections.Generic;
using UnityEngine;

public class DeadEnemiesManager : MonoBehaviour
{
    public static DeadEnemiesManager Instance { get; private set; }

    private HashSet<string> defeatedEnemies = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddDefeatedEnemy(string enemyName)
    {
        defeatedEnemies.Add(enemyName);
        Debug.Log($"Enemy {enemyName} marked as defeated.");
    }

    public bool IsEnemyDefeated(string enemyName)
    {
        return defeatedEnemies.Contains(enemyName);
    }
}