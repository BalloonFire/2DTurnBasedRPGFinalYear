using UnityEngine;
using UnityEngine.UI;

public class EnemyClickHandler : MonoBehaviour
{
    public GameObject enemyUI; // Assign this in the Inspector to the hidden UI panel.
    public GameObject buttonUI; // Display button UI

    private bool canClickEnemy = false;

    void Start()
    {
    }

    void OnMouseDown()
    {
        if (canClickEnemy) // Ensure player clicked attack button first
        {
            if (enemyUI != null) enemyUI.SetActive(true);
            if (buttonUI != null) buttonUI.SetActive(true);

            canClickEnemy = false; // Reset so multiple clicks don't trigger repeatedly
        }
    }

    public void EnableEnemyClick()
    {
        canClickEnemy = true; // Called from the Attack button
    }
}
