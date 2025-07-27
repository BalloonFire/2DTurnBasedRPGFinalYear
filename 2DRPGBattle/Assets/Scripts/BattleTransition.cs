using UnityEngine;
using Enemy.Model;

public class BattleTransition : MonoBehaviour
{
    public static BattleTransition Instance { get; private set; }

    public EnemySO CurrentEnemySO { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    public void SetEnemy(EnemySO enemy)
    {
        CurrentEnemySO = enemy;
    }
}
