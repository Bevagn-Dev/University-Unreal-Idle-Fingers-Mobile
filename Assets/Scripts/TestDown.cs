using UnityEngine;
using UnityEngine.EventSystems;

public class TestDown : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("I was clicked");
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        Debug.Log("I was left");
    }
}
