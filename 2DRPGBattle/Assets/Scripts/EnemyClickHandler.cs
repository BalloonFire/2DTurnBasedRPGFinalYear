using UnityEngine;
using Enemy;

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
        Debug.Log($"Enemy clicked: {gameObject.name} (Selectable: {selectable})");

        if (!selectable || battleHandler == null || !battleHandler.IsPlayerTurn())
        {
            Debug.Log("Cannot select - Not selectable or not player turn");
            return;
        }

        if (!enemyController.IsAlive())
        {
            Debug.Log("Cannot select - Enemy is dead");
            return;
        }

        // Deselect previous selection if necessary
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        // Always select, even if clicking the same enemy again
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