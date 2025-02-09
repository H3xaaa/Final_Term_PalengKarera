using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public Transform cameraTransform;
    public CharacterController characterController;
    public Transform characterTransform;

    private float verticalRotation = 0f;
    private bool isCursorLocked = true;

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
        HandleMouseLook();
    }

    void FollowCharacter()
    {
        if (characterTransform != null)
        {
            transform.position = characterTransform.position;
        }
    }
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
