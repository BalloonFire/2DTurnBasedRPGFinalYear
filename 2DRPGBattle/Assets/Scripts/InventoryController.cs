using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Inventory.Model;
using Inventory.UI;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryCanvas;
        [SerializeField] private UIInventoryPage inventoryUI;
        [SerializeField] private InventorySO inventoryData;
        public List<InventoryItem> startingItems = new List<InventoryItem>();
        [SerializeField] private AudioClip dropClip;
        [SerializeField] private AudioSource AudioSource;

        private bool isInventoryOpen = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            InitializeInventory();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeInventory();
            // Reassign AudioSource if missing after scene load
            if (AudioSource == null)
                AudioSource = FindObjectOfType<AudioSource>();
        }

        private void InitializeInventory()
        {
            if (inventoryCanvas != null)
            {
                inventoryUI = inventoryCanvas.GetComponentInChildren<UIInventoryPage>(true);

                if (inventoryUI == null)
                {
                    Debug.LogWarning("UIInventoryPage not found in inventory canvas.");
                    return;
                }

                // Initialize UI first
                UIReady();
                inventoryUI.InitializeInventoryUI(inventoryData.Size);

                // Then prepare inventory data
                PrepareInventory();

                // Update UI after both are ready
                UpdateInventoryUI(inventoryData.GetCurrentInventoryState());

                // Ensure canvas is initially inactive
                inventoryCanvas.SetActive(false);
                isInventoryOpen = false;
            }
        }

        private void PrepareInventory()
        {
            // Temporarily unsubscribe to prevent premature updates
            inventoryData.OnInventoryUpdated -= UpdateInventoryUI;
            inventoryData.Initialize();

            // Add starting items
            foreach (InventoryItem item in startingItems)
            {
                if (!item.IsEmpty)
                {
                    inventoryData.AddItem(item);
                }
            }

            // Re-subscribe after initialization is complete
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            if (inventoryUI == null)
            {
                Debug.LogWarning("inventoryUI is null. Skipping UI update.");
                return;
            }

            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void UIReady()
        {
            if (inventoryUI == null)
            {
                Debug.LogWarning("UIInventoryPage is null. Cannot initialize UI.");
                return;
            }

            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }
        }

        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            AudioSource.PlayOneShot(dropClip);
        }

        public void PerformAction(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(PlayerOverworldController.Instance.gameObject, inventoryItem.itemState);
                AudioManager.Instance.PlaySFX(itemAction.actionSFX);

                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    inventoryUI.ResetSelection();
            }
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
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
        }

        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();

            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} : " +
                          $"{inventoryItem.itemState[i].value} / " +
                          $"{inventoryItem.item.DefaultParametersList[i].value}");
                if (i < inventoryItem.itemState.Count - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
        }

        public void ToggleInventory()
        {
            if (inventoryUI == null)
            {
                InitializeInventory();
            }

            if (isInventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        public void OpenInventory()
        {
            if (!isInventoryOpen && inventoryCanvas != null)
            {
                isInventoryOpen = true;
                inventoryCanvas.SetActive(true);
                inventoryUI.Show();

                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                }
            }
        }

        public void CloseInventory()
        {
            if (isInventoryOpen && inventoryCanvas != null)
            {
                isInventoryOpen = false;
                inventoryCanvas.SetActive(false);
            }
        }
    }
}
