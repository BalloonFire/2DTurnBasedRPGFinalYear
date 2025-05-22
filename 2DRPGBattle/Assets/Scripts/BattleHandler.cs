using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleHandler : MonoBehaviour
{
    private bool playerTurn = true;
    private int currentPlayerIndex = 0;
    private PlayerController[] players;
    private int readyPlayers = 0;

    public GameObject TurnPlayerUI;
    public GameObject TurnEnemyUI;

    public EnemyController[] enemies;
    private int enemyIndex = -1;

    public GameObject victoryScreen;
    public GameObject defeatScreen;

    private PlayerController currentPlayer;
    private EnemyController currentEnemy;

    void Start()
    {
        players = FindObjectsOfType<PlayerController>();
        Debug.Log("Found " + players.Length + " players in battle");

        if (players.Length > 0)
        {
            EnableCurrentPlayerControls();
        }
    }

    public bool IsPlayerTurn()
    {
        return playerTurn;
    }

    public void SetCurrentPlayer(PlayerController player)
    {
        currentPlayer = player;
        Debug.Log("Selected player: " + player.playerID);
    }

    public void SetCurrentEnemy(EnemyController enemy)
    {
        currentEnemy = enemy;
        Debug.Log("Selected enemy: " + enemy.enemyID);

        // If we have both player and enemy selected, initiate attack
        if (currentPlayer != null && currentEnemy != null)
        {
            currentPlayer.Attack(currentEnemy.gameObject);
            currentPlayer = null;
            currentEnemy = null;
        }
    }

    // Modify your attack button to enable enemy selection
    public void OnAttackButtonClicked()
    {
        foreach (EnemyController enemy in enemies)
        {
            EnemyClickHandler handler = enemy.GetComponent<EnemyClickHandler>();
            if (handler != null)
            {
                handler.SetSelectable(true);
            }
        }
    }

    public void PlayerReady(PlayerController player)
    {
        readyPlayers++;
        Debug.Log(player.playerID + " is ready. Ready players: " + readyPlayers);

        if (readyPlayers >= players.Length)
        {
            StartCoroutine(ExecutePlayerAttacks());
        }
    }

    public void PlayerAttackComplete(PlayerController player)
    {
        Debug.Log(player.playerID + " attack completed");
        // This method is called by players when their attack finishes
        // The actual turn handling is managed in ExecutePlayerAttacks coroutine
    }

    public void EnemyAttackComplete(EnemyController enemy)
    {
        Debug.Log(enemy.enemyID + " attack completed");
        // Enemy turn progression is handled in ProcessEnemyAttack coroutine
    }

    private IEnumerator ExecutePlayerAttacks()
    {
        foreach (PlayerController player in players)
        {
            if (player.IsAlive())
            {
                EnemyController target = GetNextLivingEnemy();
                if (target != null)
                {
                    player.Attack(target.gameObject);
                    yield return new WaitForSeconds(1.5f); // Wait for attack animation
                }
            }
        }

        readyPlayers = 0;
        TogglePlayerTurn();
    }

    public void CheckEnemyDefeated()
    {
        if (AllEnemiesDefeated())
        {
            ShowVictoryScreen();
        }
    }

    private EnemyController GetNextLivingEnemy()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                return enemy;
            }
        }
        return null;
    }

    public void TogglePlayerTurn()
    {
        if (AllPlayersDefeated())
        {
            ShowDefeatScreen();
            return;
        }

        if (AllEnemiesDefeated())
        {
            ShowVictoryScreen();
            return;
        }

        playerTurn = !playerTurn;

        if (playerTurn)
        {
            TurnPlayerUI.SetActive(true);
            TurnEnemyUI.SetActive(false);
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
            EnableCurrentPlayerControls();
        }
        else
        {
            TurnPlayerUI.SetActive(false);
            TurnEnemyUI.SetActive(true);
            EnemyTurn();
        }
    }
    public void ContinueEnemyTurn()
    {
        // This gets called when an enemy finishes their attack animation
        EnemyTurn(); // Move to the next enemy
    }

    private void EnemyTurn()
    {
        enemyIndex++;

        if (enemyIndex >= enemies.Length)
        {
            // All enemies have attacked, switch back to player turn
            enemyIndex = -1;
            TogglePlayerTurn();
            return;
        }

        // Skip dead enemies
        if (!enemies[enemyIndex].IsAlive())
        {
            EnemyTurn();
            return;
        }

        // Process this enemy's attack
        StartCoroutine(ProcessEnemyAttack(enemies[enemyIndex]));
    }


    IEnumerator ProcessEnemyAttack(EnemyController enemy)
    {
        // Early exit if enemy is invalid or dead
        if (enemy == null || !enemy.IsAlive())
        {
            EnemyTurn();
            yield break;
        }

        PlayerController target = GetRandomLivingPlayer();

        // If no living players, end battle
        if (target == null)
        {
            ShowDefeatScreen();
            yield break;
        }

        // Store original position
        Vector3 originalPos = enemy.transform.position;

        try
        {
            // Execute attack and wait for completion
            yield return StartCoroutine(enemy.ExecuteAttack(target.gameObject));
        }
        finally
        {
            // Ensure enemy always returns to original position
            enemy.transform.position = originalPos;

            // Only proceed to next enemy if battle isn't over
            if (!AllPlayersDefeated() && !AllEnemiesDefeated())
            {
                EnemyTurn();
            }
        }
    }


    private PlayerController GetRandomLivingPlayer()
    {
        List<PlayerController> livingPlayers = new List<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player.IsAlive())
            {
                livingPlayers.Add(player);
            }
        }
        return livingPlayers.Count > 0 ? livingPlayers[Random.Range(0, livingPlayers.Count)] : null;
    }

    private bool AllPlayersDefeated()
    {
        foreach (PlayerController player in players)
        {
            if (player.IsAlive())
            {
                return false;
            }
        }
        return true;
    }

    private bool AllEnemiesDefeated()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                return false;
            }
        }
        return true;
    }

    private void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
        TurnPlayerUI.SetActive(false);
        TurnEnemyUI.SetActive(false);
        StartCoroutine(WaitAndLoadMenu());
    }

    public void CheckGameOver()
    {
        if (AllPlayersDefeated())
        {
            ShowDefeatScreen();
        }
    }

    private void ShowDefeatScreen()
    {
        defeatScreen.SetActive(true);
        TurnPlayerUI.SetActive(false);
        TurnEnemyUI.SetActive(false);
        StartCoroutine(WaitAndLoadMenu());
    }

    private IEnumerator WaitAndLoadMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadSceneAsync(0);
    }

    private void EnableCurrentPlayerControls()
    {
        // Implementation for enabling specific player controls
        // Example: players[currentPlayerIndex].EnableControls();
    }
}