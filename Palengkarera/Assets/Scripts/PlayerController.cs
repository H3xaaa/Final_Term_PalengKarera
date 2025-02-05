using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public MobileJoystick joystick; 
    public Transform cameraTransform; 
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float movementDamping = 5f; 

    private CharacterController characterController;
    private Vector3 lastMoveDirection = Vector3.zero;
    private float rotationY; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotationY = transform.eulerAngles.y;
    }

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

  
        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        moveInput = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * moveInput; 

        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = moveInput.normalized * moveSpeed;
        }
        else
        {
            lastMoveDirection = Vector3.Lerp(lastMoveDirection, Vector3.zero, Time.deltaTime * movementDamping);
        }

   
        characterController.Move(lastMoveDirection * Time.deltaTime);

  
        if (lastMoveDirection.magnitude > 0.1f)
        {
  
            Quaternion toRotation = Quaternion.LookRotation(lastMoveDirection);
            rotationY = Mathf.LerpAngle(rotationY, cameraTransform.eulerAngles.y, rotationSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.Euler(0, rotationY, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); 
        }
    }
}
