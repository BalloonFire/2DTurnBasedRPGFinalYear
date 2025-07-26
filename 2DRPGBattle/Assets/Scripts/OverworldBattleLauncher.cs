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
        GameDataCarrier.Instance.SetSelectedPlayers(chosenParty);
        SceneManager.LoadScene("BattleSceneNameHere");
    }
}
