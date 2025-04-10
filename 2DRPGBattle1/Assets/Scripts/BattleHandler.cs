using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleHandler : MonoBehaviour
{
    bool playerTurn = true;

    public GameObject PlayerPanel;
    public GameObject TurnPlayerUI;
    public GameObject TurnEnemyUI;

    public EnemyController[] enemies;
    private int enemyIndex = -1;

    public GameObject victoryScreen; // UI for winning
    public GameObject defeatScreen;  // UI for losing

    private PlayerController playerController;

    void Start()
    {
        PlayerPanel.SetActive(true);
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void TogglePlayerTurn()
    {
        if (playerController.health <= 0)
        {
            ShowDefeatScreen();
            return; // Stop execution to prevent UI from updating
        }

        if (AllEnemiesDefeated())
        {
            ShowVictoryScreen();
            return;
        }

        playerTurn = !playerTurn;

        if (playerTurn)
        {
            PlayerPanel.SetActive(true);
            TurnPlayerUI.SetActive(true);
            TurnEnemyUI.SetActive(false);
        }
        else
        {
            PlayerPanel.SetActive(false);
            TurnEnemyUI.SetActive(true);
            TurnPlayerUI.SetActive(false);
            EnemyTurn();
        }
    }

    public void EnemyTurn()
    {
        if (playerController.health <= 0)
        {
            ShowDefeatScreen();
            return; // Stop execution to prevent UI from updating
        }

        if (AllEnemiesDefeated())
        {
            ShowVictoryScreen();
            return;
        }

        enemyIndex++;

        if (enemyIndex >= enemies.Length) // Reset cycle correctly
        {
            enemyIndex = -1; // Reset before switching turns
            TogglePlayerTurn();
            return;
        }

        StartCoroutine(WaitForAttack(enemies[enemyIndex]));
    }

    public void NextEnemy()
    {
        EnemyTurn();
    }

    IEnumerator WaitForAttack(EnemyController ec)
    {
        yield return new WaitForSeconds(2f);
        if (ec.health > 0) // Only attack if enemy is alive
        {
            ec.Attack();
        }
        else
        {
            EnemyTurn(); // Skip dead enemy and move to the next
        }
    }

    private bool AllEnemiesDefeated()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.health > 0)
            {
                return false; // At least one enemy is still alive
            }
        }
        return true; // All enemies are dead
    }

    private void ShowVictoryScreen()
    {
        Debug.Log("Victory! All enemies defeated.");
        victoryScreen.SetActive(true);
        TurnPlayerUI.SetActive(false);
        TurnEnemyUI.SetActive(false);
        PlayerPanel.SetActive(false);

        playerTurn = false;

        StartCoroutine(WaitAndLoadMenu());
    }

    public void CheckGameOver()
    {
        if (playerController.health <= 0)
        {
            ShowDefeatScreen();
        }
    }

    private void ShowDefeatScreen()
    {
        if (defeatScreen.activeSelf) return; // Prevent duplicate execution

        Debug.Log("Defeat! Player has been defeated.");
        defeatScreen.SetActive(true);
        TurnPlayerUI.SetActive(false);
        TurnEnemyUI.SetActive(false);
        PlayerPanel.SetActive(false);

        playerTurn = false;

        StartCoroutine(WaitAndLoadMenu());
    }

    private IEnumerator WaitAndLoadMenu()
    {
        yield return new WaitForSeconds(7f);
        SceneManager.LoadSceneAsync(0); // Load the menu screen
    }
}
