using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    bool playerTurn = true;

    public GameObject PlayerPanel;

    public GameObject TurnPlayerUI;

    public GameObject TurnEnemyUI;

    public EnemyController[] enemies;
    private int enemyIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TogglePlayerTurn()
    {
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
        yield return new WaitForSeconds(3f);
        ec.Attack1();
    }
}
