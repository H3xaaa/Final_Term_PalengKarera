using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstPersonCam : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public Transform cameraTransform;
    public Transform characterTransform;
    public RectTransform touchPanel;

    private float verticalRotation = 0f;
    private bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        FollowCharacter();
        HandleMouseDrag();
    }

    void FollowCharacter()
    {
        if (characterTransform != null)
        {
            transform.position = characterTransform.position;
        }
    }
    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0) && IsPointerOverUIElement(touchPanel))
        {
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        if (isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    bool IsPointerOverUIElement(RectTransform panel)
    {
        return EventSystem.current.IsPointerOverGameObject(); 
    }
}
