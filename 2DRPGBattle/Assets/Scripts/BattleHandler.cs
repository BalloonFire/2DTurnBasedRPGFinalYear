using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Enemy;
using UnityEngine.SceneManagement;
using System.Linq;

public class BattleHandler : MonoBehaviour
{
    [Header("Battle Settings")]
    public bool playerTurn = true;
    public float enemyTurnDelay = 1.5f;
    public float endBattleDelay = 10f;

    [Header("Return Scene (Optional)")]
    [Tooltip("Leave empty to use automatic scene transition, or specify scene name to return to")]
    public string returnScene = "";

    [Header("Attack Panel")]
    public GameObject attackConfirmationPanel;

    [Header("UI notification")]
    public GameObject playerTurns;
    public GameObject enemyTurns;
    public GameObject winUI;
    public GameObject loseUI;

    private PlayerController currentPlayer;
    private EnemyController currentEnemy;
    private bool gameEnded = false;

    public static BattleHandler Instance { get; private set; }
    private List<PlayerController> allPlayers = new List<PlayerController>();
    private HashSet<PlayerController> playersWhoAttacked = new HashSet<PlayerController>();

    void Start()
    {
        Instance = this;

        // Initialize UI states first
        attackConfirmationPanel.SetActive(false);
        winUI.SetActive(false);
        loseUI.SetActive(false);
        playerTurns.SetActive(false);
        enemyTurns.SetActive(false);

        // Wait one frame to ensure all players are initialized
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null; // Wait one frame

        // Now find and initialize players
        allPlayers.AddRange(FindObjectsOfType<PlayerController>());
        DisableAllPlayerSelection();
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
        yield return new WaitForSeconds(enemyTurnDelay); // Initial delay

        // Get all enemies and sort them by their EnemyID
        var allEnemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
        allEnemies.Sort((a, b) => a.enemyID.CompareTo(b.enemyID));

        var players = FindObjectsOfType<PlayerController>();
        List<PlayerController> alivePlayers = new List<PlayerController>(players.Where(p => p.IsAlive()));

        if (alivePlayers.Count == 0)
        {
            Debug.Log("All players defeated!");
            CheckGameOver();
            yield break;
        }

        // Attack in strict order with delays
        foreach (var enemy in allEnemies)
        {
            if (!enemy.IsAlive()) continue;

            Debug.Log($"{enemy.enemyID} preparing to attack...");

            // Wait for enemy to complete their attack sequence
            yield return enemy.ExecuteAttack(players);

            // Add delay between enemy attacks
            yield return new WaitForSeconds(enemyTurnDelay);

            CheckGameOver();
            if (gameEnded) yield break;
        }

        if (!gameEnded) BeginPlayerTurn();
    }

    public bool IsPlayerTurn() => playerTurn;

    public void CheckGameOver()
    {
        if (gameEnded) return;

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
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                allEnemiesDead = false;
                break;
            }
        }

        if (allPlayersDead)
        {
            gameEnded = true;
            Debug.Log("Game Over - Players Defeated!");
            loseUI.SetActive(true);
            StartCoroutine(EndBattle(false));
        }
        else if (allEnemiesDead)
        {
            gameEnded = true;
            Debug.Log("Victory - All Enemies Defeated!");

            // Only record the original overworld enemy
            if (DeadEnemiesManager.Instance != null && BattleTransition.Instance != null)
            {
                string overworldEnemyName = BattleTransition.Instance.OverworldEnemyName;
                if (!string.IsNullOrEmpty(overworldEnemyName))
                {
                    Debug.Log($"Marking overworld enemy as defeated: {overworldEnemyName}");
                    DeadEnemiesManager.Instance.AddDefeatedEnemy(overworldEnemyName);
                }
                else
                {
                    Debug.LogWarning("No overworld enemy name recorded!");
                }
            }
            else
            {
                Debug.LogError("Critical manager instances are null!");
            }

            winUI.SetActive(true);
            StartCoroutine(EndBattle(true));
        }
    }

    private IEnumerator EndBattle(bool won)
    {
        // Disable all interactions
        DisableAllPlayerSelection();
        SetAllEnemiesSelectable(false);
        playerTurns.SetActive(false);
        enemyTurns.SetActive(false);
        attackConfirmationPanel.SetActive(false);

        // Wait to show the result
        yield return new WaitForSeconds(endBattleDelay);

        // Determine which scene to return to
        string sceneToLoad;

        if (!string.IsNullOrEmpty(returnScene))
        {
            // Use the manually specified scene
            sceneToLoad = returnScene;
        }
        else if (SceneTracker.Instance != null)
        {
            // Use the SceneTracker's previous scene
            sceneToLoad = SceneTracker.Instance.GetPreviousScene();
        }
        else
        {
            // Fallback to menu scene
            sceneToLoad = "MenuScenes";
        }

        SceneManager.LoadScene(sceneToLoad);
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