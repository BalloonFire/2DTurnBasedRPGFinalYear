using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.UI;
using Inventory.Model;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryPage inventoryUI;

        [SerializeField]
        private InventorySO inventoryData;

        public List<InventoryItem> startingItems = new List<InventoryItem>();

        private bool isInventoryOpen = false;

        private void Start()
        {
            UIReady();
            PrepareInventory();
        }

        private void PrepareInventory()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in startingItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void UIReady()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            // Your item action implementation
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            if (itemIndex_1 >= 0 && itemIndex_1 < inventoryData.Size &&
                itemIndex_2 >= 0 && itemIndex_2 < inventoryData.Size)
            {
                inventoryData.SwapItems(itemIndex_1, itemIndex_2);
            }
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, item.Description);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
        }

        // Main toggle method (works with keyboard input)
        public void ToggleInventory()
        {
            if (isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        // Public method specifically for opening the inventory
        public void OpenInventory()
        {
            if (!isInventoryOpen)
            {
                isInventoryOpen = true;
                inventoryUI.Show();

                // Refresh UI when opening
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key,
                                       item.Value.item.ItemImage,
                                       item.Value.quantity);
                }
            }
        }

        // Public method specifically for closing the inventory
        public void CloseInventory()
        {
            if (isInventoryOpen)
            {
                isInventoryOpen = false;
                inventoryUI.Hide();
            }
        }
    }
}