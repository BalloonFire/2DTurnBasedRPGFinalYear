using UnityEngine;
using Enemy.Model;

public class BattleTransition : MonoBehaviour
{
    public static BattleTransition Instance { get; private set; }
    public EnemySO CurrentEnemySO { get; private set; }
    public string OverworldEnemyName { get; private set; }

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

    public void SetEnemy(EnemySO enemyData, string overworldEnemyName)
    {
        CurrentEnemySO = enemyData;
        OverworldEnemyName = overworldEnemyName;
        Debug.Log($"BattleTransition: Set enemy {overworldEnemyName} with data {enemyData.enemyName}");
    }
}