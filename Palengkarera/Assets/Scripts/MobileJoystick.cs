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
    private bool isKeyboardOverride = false;

    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;
    public Vector2 Direction => inputVector;

    void Update()
    {
        // Keyboard Sync: Force joystick movement when pressing W, A, S, D
        Vector2 keyboardInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) keyboardInput.y += 1f;
        if (Input.GetKey(KeyCode.S)) keyboardInput.y -= 1f;
        if (Input.GetKey(KeyCode.A)) keyboardInput.x -= 1f;
        if (Input.GetKey(KeyCode.D)) keyboardInput.x += 1f;

        if (keyboardInput != Vector2.zero)
        {
            isKeyboardOverride = true;
            inputVector = Vector2.ClampMagnitude(keyboardInput, handleLimit);
            joystickHandle.anchoredPosition = inputVector * (joystickBackground.sizeDelta / 2);
        }
        else if (isKeyboardOverride)
        {
            isKeyboardOverride = false;
            inputVector = Vector2.zero;
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isKeyboardOverride = false;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        isKeyboardOverride = false;

        Vector2 position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out position);

        position = Vector2.ClampMagnitude(position / (joystickBackground.sizeDelta / 2), handleLimit);
        inputVector = position;

        joystickHandle.anchoredPosition = position * (joystickBackground.sizeDelta / 2);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isKeyboardOverride = false;
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }
}
