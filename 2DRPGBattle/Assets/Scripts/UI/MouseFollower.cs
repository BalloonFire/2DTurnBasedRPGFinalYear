using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private UIInventoryItem inventoryItemUI;

    public void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        inventoryItemUI = GetComponentInChildren<UIInventoryItem>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        inventoryItemUI.SetData(sprite, quantity);
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Toggle(bool val)
    {
        Debug.Log($"Item toggled {val}");
        gameObject.SetActive(val);
    }
}
