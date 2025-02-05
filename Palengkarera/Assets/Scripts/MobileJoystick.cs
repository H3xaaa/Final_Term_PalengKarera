using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform joystickBackground; 
    public RectTransform joystickHandle; 
    public float handleLimit = 1f; 

    private Vector2 inputVector = Vector2.zero;

    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;
    public Vector2 Direction => inputVector;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out position);

        position = Vector2.ClampMagnitude(position / (joystickBackground.sizeDelta / 2), handleLimit);
        inputVector = position;

        joystickHandle.anchoredPosition = position * (joystickBackground.sizeDelta / 2);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero; 
    }
}
