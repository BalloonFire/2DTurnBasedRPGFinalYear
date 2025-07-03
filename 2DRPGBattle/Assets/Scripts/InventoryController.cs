using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPage inventoryUI;

    public int inventorySize = 10;

    private void Start()
    {
        inventoryUI.InitializeInventoryUI(inventorySize);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryUI();
        }
    }

    // This method is public so it can be used in the Unity Button's OnClick
    public void ToggleInventoryUI()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("Inventory UI reference is missing.");
            return;
        }

        if (inventoryUI.isActiveAndEnabled == false)
        {
            inventoryUI.Show();
        }
        else
        {
            inventoryUI.Hide();
        }
    }
}
