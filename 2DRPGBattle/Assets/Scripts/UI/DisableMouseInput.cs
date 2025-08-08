using UnityEngine;
using UnityEngine.EventSystems;

public class DisableMouseInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId == -1)  // mouse pointer ID is -1 in Unity
        {
            // Ignore mouse input by not doing anything
            eventData.pointerPress = null;  // prevent event propagation
            return;
        }
        // Otherwise handle normally (if you want)
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == -1)
        {
            eventData.pointerPress = null;
            return;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerId == -1)
        {
            eventData.pointerPress = null;
            return;
        }
    }
}
