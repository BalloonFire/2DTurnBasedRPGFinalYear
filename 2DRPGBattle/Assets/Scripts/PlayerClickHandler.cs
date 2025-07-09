using UnityEngine;
using Player;

public class PlayerClickHandler : MonoBehaviour
{
    [Header("Selection UI")]
    public GameObject playerUI;
    public GameObject selectionRing;

    private PlayerController player;
    private BattleHandler battleHandler;
    private Collider2D col;

    private bool isSelectable = true;

    void Start()
    {
        player = GetComponent<PlayerController>();
        battleHandler = FindObjectOfType<BattleHandler>();
        col = GetComponent<Collider2D>();

        if (col != null) col.enabled = true;
        SetSelected(false);
    }

    void OnMouseDown()
    {
        if (!isSelectable || battleHandler == null || !battleHandler.IsPlayerTurn()) return;
        if (!player.IsAlive()) return;
        if (player.IsAttacking()) return;

        battleHandler.DeselectAllPlayers();
        battleHandler.SetCurrentPlayer(player);
        SetSelected(true);
        player.UpdateAttackButtons();
    }

    public void SetSelected(bool selected)
    {
        if (playerUI != null)
            playerUI.SetActive(selected);

        if (selectionRing != null)
            selectionRing.SetActive(selected);
    }

    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;

        if (col != null)
            col.enabled = selectable;

        // Optional: Visual feedback for disabled player
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            sprite.color = selectable ? Color.white : Color.gray;
    }

    public bool IsSelectable()
    {
        return isSelectable;
    }
}
