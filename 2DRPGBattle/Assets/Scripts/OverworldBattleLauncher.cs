using Player.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldBattleLauncher : MonoBehaviour
{
    public List<PlayerSO> chosenParty; // Fill from UI or script

    public void StartBattle()
    {
        // Save selected characters for battle
        GameDataCarrier.Instance.SetSelectedPlayers(chosenParty);

        // Track the scene and player position
        SceneTracker.Instance.SetPreviousScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        SceneTracker.Instance.SetPlayerPosition(PlayerOverworldController.Instance.transform.position);

        // Load the battle scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleTest");
    }
}
