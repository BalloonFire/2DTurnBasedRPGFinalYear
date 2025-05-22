using UnityEngine;

public class PlayerClickHandler : MonoBehaviour
{
    public GameObject playerUI;      // Assign in Inspector: this player's UI panel
    public GameObject selectionRing; // Assign in Inspector: green circle
    private static PlayerClickHandler currentlySelected;

    private PlayerController playerController;
    private BattleHandler battleHandler;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        battleHandler = FindObjectOfType<BattleHandler>();
        Deselect(); // Start deselected
    }

    void OnMouseDown()
    {
        // Only allow selection during player turn
        if (battleHandler == null || !battleHandler.IsPlayerTurn()) return;

        // Don't select dead players
        if (!playerController.IsAlive()) return;

        // Deselect previously selected player
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        // Select this player
        Select();
    }

    public void Select()
    {
        if (playerUI != null) playerUI.SetActive(true);
        if (selectionRing != null) selectionRing.SetActive(true);

        currentlySelected = this;

        // Notify BattleHandler
        if (battleHandler != null)
        {
            battleHandler.SetCurrentPlayer(playerController);
        }
    }

    public void Deselect()
    {
        if (playerUI != null) playerUI.SetActive(false);
        if (selectionRing != null) selectionRing.SetActive(false);
    }

    public static PlayerController GetSelectedPlayer()
    {
        return currentlySelected?.playerController;
    }
}