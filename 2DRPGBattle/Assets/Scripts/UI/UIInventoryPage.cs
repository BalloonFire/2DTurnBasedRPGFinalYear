using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryItem itemPrelab;

        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private UIInventoryDescription itemDescription;

        [SerializeField]
        private MouseFollower mouseFollower;

        List<UIInventoryItem> listItems = new List<UIInventoryItem>();

        private int currentDragItemIndex = -1;

        public event Action<int> OnDescriptionRequested,
        OnItemActionRequested,
        OnStartDragging;

        public event Action<int, int> OnSwapItems;

        private void Awake()
        {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }

        public void InitializeInventoryUI(int inventorysize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                UIInventoryItem uiItem = Instantiate(itemPrelab, contentPanel, false);
                uiItem.transform.localScale = Vector3.one;
                listItems.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        public void UpdateData(int itemIndex,
                           Sprite itemImage, int itemQuantity)
        {
            if (listItems.Count > itemIndex)
            {
                listItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {

        }

        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDragItem();
        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            int index = listItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnSwapItems?.Invoke(currentDragItemIndex, index);
            HandleItemSelection(inventoryItemUI);

        }

        private void ResetDragItem()
        {
            mouseFollower.Toggle(false);
            currentDragItemIndex = -1;
        }

        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            int index = listItems.IndexOf(inventoryItemUI);
            if (index == -1 || listItems[index].IsEmpty)
                return;
            currentDragItemIndex = index;
            HandleItemSelection(inventoryItemUI); // Keep selection during drag
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }


        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            int index = listItems.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            OnDescriptionRequested?.Invoke(index);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectItems();
        }

        private void DeselectItems()
        {
            foreach (UIInventoryItem item in listItems)
            {
                item.Deselect();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ResetDragItem();
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectItems();
            listItems[itemIndex].Select();
        }

        internal void ResetAllItems()
        {
            foreach (var item in listItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }
    }
}