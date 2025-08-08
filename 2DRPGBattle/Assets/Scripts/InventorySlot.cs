using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private WeaponInfo weaponInfo;

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Find ActiveInventory in scene (or assign it via inspector)
        ActiveInventory activeInventory = FindObjectOfType<ActiveInventory>();
        if (activeInventory != null)
        {
            int index = transform.GetSiblingIndex();
            activeInventory.ToggleActiveHighlight(index);
        }
    }
}
