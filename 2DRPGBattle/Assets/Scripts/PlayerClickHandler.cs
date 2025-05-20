using UnityEngine;

public class PlayerClickHandler : MonoBehaviour
{
    public GameObject playerUI;      // Assign in Inspector: this player's UI panel
    public GameObject selectionRing; // Assign in Inspector: green circle
    private static PlayerClickHandler currentlySelected;

    void OnMouseDown()
    {
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

        // Optional: also tell BattleHandler which player is selected
        BattleHandler battleHandler = FindObjectOfType<BattleHandler>();
        if (battleHandler != null)
        {
            battleHandler.SetCurrentPlayer(gameObject.GetComponent<PlayerController>());
        }
    }

    public void Deselect()
    {
        if (playerUI != null) playerUI.SetActive(false);
        if (selectionRing != null) selectionRing.SetActive(false);
    }
}
