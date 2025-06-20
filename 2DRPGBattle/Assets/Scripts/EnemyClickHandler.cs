using UnityEngine;

public class EnemyClickHandler : MonoBehaviour
{
    [Header("Selection UI")]
    public GameObject enemyUI;
    public GameObject selectionIndicator;

    private static EnemyClickHandler currentlySelected;

    private EnemyController enemyController;
    private BattleHandler battleHandler;
    private bool selectable = false;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        battleHandler = FindObjectOfType<BattleHandler>();
        Deselect(); // Start deselected
    }

    void OnMouseDown()
    {
        // Only allow selection when enabled and during player turn
        if (!selectable || battleHandler == null || !battleHandler.IsPlayerTurn()) return;

        // Don't select dead enemies
        if (!enemyController.IsAlive()) return;

        // Deselect previously selected enemy
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        // Select this enemy
        Select();
    }

    public void Select()
    {
        if (selectionIndicator != null) selectionIndicator.SetActive(true);
        currentlySelected = this;
        battleHandler?.SetCurrentEnemy(enemyController);
    }

    public void Deselect()
    {
        if (selectionIndicator != null) selectionIndicator.SetActive(false);
    }

    public void SetSelectable(bool canSelect)
    {
        selectable = canSelect;
    }

    public static EnemyController GetSelectedEnemy()
    {
        return currentlySelected?.enemyController;
    }
}