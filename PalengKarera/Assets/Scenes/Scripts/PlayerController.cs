using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public MobileJoystick joystick; // Reference to the joystick
    public Transform cameraTransform; // Camera reference for movement direction
    public float normalSpeed = 5f; // Movement speed while walking
    public float runSpeed = 15f; // Movement speed while running
    public float movementDamping = 5f; // Damping for movement transition
    public float rotationSpeed = 10f; // Speed for smooth rotation

    public float staminaMax = 100f; // Maximum stamina value
    public float staminaDepletionRate = 25f; // Rate at which stamina depletes while running
    public float staminaRechargeRate = 25f; // Rate at which stamina recharges while not running

    public Image staminaBar; // UI element representing the stamina bar
    public Button runButton; // UI button used for running

    private CharacterController characterController;
    private Vector3 lastMoveDirection = Vector3.zero;
    private float currentStamina;
    private float rotationY;
    private bool isHoldingRun = false; // Tracks if the button is held
    private bool isRunning = false; // Tracks if the player is running

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentStamina = staminaMax; // Initialize stamina to full
        rotationY = transform.eulerAngles.y;

        // Add EventTrigger for the Run button
        EventTrigger trigger = runButton.gameObject.AddComponent<EventTrigger>();

        // Detect button press (start holding)
        EventTrigger.Entry pressEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pressEntry.callback.AddListener((data) => isHoldingRun = true);
        trigger.triggers.Add(pressEntry);

        // Detect button release (stop holding)
        EventTrigger.Entry releaseEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        releaseEntry.callback.AddListener((data) => isHoldingRun = false);
        trigger.triggers.Add(releaseEntry);
    }

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Calculate the movement direction relative to the camera
        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        moveInput = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * moveInput;

        // Determine the movement speed
        if (moveInput.magnitude > 0.1f)
        {
            isRunning = isHoldingRun && currentStamina > 0; // Run only if holding and stamina is available
            lastMoveDirection = moveInput.normalized * (isRunning ? runSpeed : normalSpeed);
        }
        else
        {
            lastMoveDirection = Vector3.Lerp(lastMoveDirection, Vector3.zero, Time.deltaTime * movementDamping);
        }

        // Move the player
        characterController.Move(lastMoveDirection * Time.deltaTime);

        // Handle stamina logic
        if (isRunning)
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            if (currentStamina < 0)
                currentStamina = 0;
        }
        else if (!isHoldingRun && currentStamina < staminaMax)
        {
            currentStamina += staminaRechargeRate * Time.deltaTime;
            if (currentStamina > staminaMax)
                currentStamina = staminaMax;
        }

        // Update the stamina bar UI
        staminaBar.fillAmount = currentStamina / staminaMax;

        // Handle player rotation
        if (lastMoveDirection.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(lastMoveDirection);
            rotationY = Mathf.LerpAngle(rotationY, cameraTransform.eulerAngles.y, rotationSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.Euler(0, rotationY, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}