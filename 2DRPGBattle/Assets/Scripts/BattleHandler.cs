using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Enemy;

public class BattleHandler : MonoBehaviour
{
    [Header("Battle Settings")]
    public bool playerTurn = true;
    public float enemyTurnDelay = 1.5f;

    [Header("Attack Panel")]
    public GameObject attackConfirmationPanel;

    [Header("UI notification")]
    public GameObject playerTurns;
    public GameObject enemyTurns;
    public GameObject winUI;
    public GameObject loseUI;

    private PlayerController currentPlayer;
    private EnemyController currentEnemy;

    private List<PlayerController> allPlayers = new List<PlayerController>();
    private HashSet<PlayerController> playersWhoAttacked = new HashSet<PlayerController>();

    void Start()
    {
        attackConfirmationPanel.SetActive(false); // Hide panel at start
        allPlayers.AddRange(FindObjectsOfType<PlayerController>());
        DisableAllPlayerSelection(); // Prevent clicks before turn starts
        BeginPlayerTurn();
    }

    public void SetCurrentPlayer(PlayerController player)
    {
        if (!IsPlayerTurn() || playersWhoAttacked.Contains(player)) return;

        currentPlayer = player;
        DeselectAllPlayers();
        player.UpdateAttackButtons();
        attackConfirmationPanel.SetActive(false);
    }

    public void PlayerReady(PlayerController player)
    {
        if (!IsPlayerTurn() || playersWhoAttacked.Contains(player)) return;

        SetAllEnemiesSelectable(true);
        UpdateAttackInfo();
    }

    public void SetCurrentEnemy(EnemyController enemy)
    {
        if (!IsPlayerTurn()) return;

        currentEnemy = enemy;
        UpdateAttackInfo();

        if (currentPlayer != null && currentPlayer.attackSelected != -1 && currentEnemy != null && currentEnemy.IsAlive())
        {
            attackConfirmationPanel.SetActive(true);
        }
        else
        {
            attackConfirmationPanel.SetActive(false);
        }
    }

    private void UpdateAttackInfo()
    {
        if (currentPlayer == null || currentPlayer.attackSelected == -1) return;

        string attackName = "";
        int minDmg = 0, maxDmg = 0, critChance = 0;

        switch (currentPlayer.attackSelected)
        {
            case 0:
                attackName = "Basic Attack";
                minDmg = currentPlayer.playerData.minDmgAtk;
                maxDmg = currentPlayer.playerData.maxDmgAtk;
                critChance = currentPlayer.playerData.critChanceAtk;
                break;
            case 1:
                attackName = "Skill Attack";
                minDmg = currentPlayer.playerData.minDmgSkill;
                maxDmg = currentPlayer.playerData.maxDmgSkill;
                critChance = currentPlayer.playerData.critChanceSkill;
                break;
            case 2:
                attackName = "Ultimate Attack";
                minDmg = currentPlayer.playerData.minDmgUltimate;
                maxDmg = currentPlayer.playerData.maxDmgUltimate;
                critChance = currentPlayer.playerData.critChanceUltimate;
                break;
        }

        string targetName = currentEnemy != null ? currentEnemy.enemyID.ToString() : "No Target";
        Debug.Log($"{attackName}\nTarget: {targetName}\nDamage: {minDmg}-{maxDmg}\nCrit: {critChance}%");
    }

    public void PlayerAttack()
    {
        if (!IsPlayerTurn() || currentEnemy == null || !currentEnemy.IsAlive()) return;
        if (playersWhoAttacked.Contains(currentPlayer)) return;

        ConfirmAttack();
    }

    private void ConfirmAttack()
    {
        if (currentPlayer != null && currentEnemy != null && currentEnemy.IsAlive())
        {
            currentPlayer.Attack(currentEnemy.gameObject);
            playersWhoAttacked.Add(currentPlayer);
            currentPlayer.GetComponent<PlayerClickHandler>().SetSelectable(false);
        }

        CancelAttack();

        if (AllAlivePlayersAttacked())
        {
            EndPlayerTurn();
        }
    }

    private bool AllAlivePlayersAttacked()
    {
        int alivePlayerCount = 0;

        foreach (var player in allPlayers)
        {
            if (player.IsAlive())
                alivePlayerCount++;
        }

        return playersWhoAttacked.Count >= alivePlayerCount;
    }

    private void CancelAttack()
    {
        SetAllEnemiesSelectable(false);

        if (currentPlayer != null)
        {
            currentPlayer.attackSelected = -1;
            currentPlayer.UpdateAttackButtons();
        }

        currentEnemy = null;
        attackConfirmationPanel.SetActive(false);
    }

    private void SetAllEnemiesSelectable(bool selectable)
    {
        foreach (var enemy in FindObjectsOfType<EnemyClickHandler>())
        {
            enemy.SetSelectable(selectable);
            if (!selectable) enemy.Deselect();
        }
    }

    public void DeselectAllPlayers()
    {
        foreach (var player in FindObjectsOfType<PlayerClickHandler>())
        {
            player.SetSelected(false);
        }
    }

    private void DisableAllPlayerSelection()
    {
        foreach (var clicker in FindObjectsOfType<PlayerClickHandler>())
        {
            clicker.SetSelectable(false);
        }
    }

    private void EnableAllPlayerSelection()
    {
        foreach (var clicker in FindObjectsOfType<PlayerClickHandler>())
        {
            var player = clicker.GetComponent<PlayerController>();
            if (player.IsAlive() && !playersWhoAttacked.Contains(player))
                clicker.SetSelectable(true);
        }
    }

    public void BeginPlayerTurn()
    {
        playerTurn = true;
        playersWhoAttacked.Clear();
        EnableAllPlayerSelection();
        playerTurns.SetActive(true);
        enemyTurns.SetActive(false);
        Debug.Log("Player turn begins!");
    }

    public void EndPlayerTurn()
    {
        playerTurn = false;
        DisableAllPlayerSelection();
        attackConfirmationPanel.SetActive(false);
        playerTurns.SetActive(false);
        enemyTurns.SetActive(true);
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy turn begins!");
        yield return new WaitForSeconds(enemyTurnDelay);

        var allEnemies = FindObjectsOfType<EnemyController>();
        var players = FindObjectsOfType<PlayerController>();

        // Create array of alive players
        List<PlayerController> alivePlayers = new List<PlayerController>();
        foreach (var p in players)
        {
            if (p.IsAlive()) alivePlayers.Add(p);
        }

        if (alivePlayers.Count == 0)
        {
            Debug.Log("All players defeated!");
            yield break;
        }

        foreach (var enemy in allEnemies)
        {
            if (!enemy.IsAlive()) continue;

            // Pass the array of all players to the enemy
            yield return StartCoroutine(enemy.ExecuteAttack(players));
        }

        BeginPlayerTurn();
    }

    public bool IsPlayerTurn() => playerTurn;

    public void CheckGameOver()
    {
        bool allPlayersDead = true;
        foreach (var player in allPlayers)
        {
            if (player.IsAlive())
            {
                allPlayersDead = false;
                break;
            }
        }

        bool allEnemiesDead = true;
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            if (enemy.IsAlive())
            {
                allEnemiesDead = false;
                break;
            }
        }

        if (allPlayersDead)
        {
            Debug.Log("Game Over - Players Defeated!");
            loseUI.SetActive(true);
        }
        else if (allEnemiesDead)
        {
            Debug.Log("Victory - All Enemies Defeated!");
            winUI.SetActive(true);
        }
    }

    public void NotifyPlayerFinished(PlayerController player)
    {
        if (!playersWhoAttacked.Contains(player))
            playersWhoAttacked.Add(player);

        player.GetComponent<PlayerClickHandler>().SetSelectable(false);

        if (AllAlivePlayersAttacked())
        {
            EndPlayerTurn();
        }
    }
}