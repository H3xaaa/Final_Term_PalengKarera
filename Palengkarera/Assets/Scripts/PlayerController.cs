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

    private Rigidbody rb;
    private Vector3 lastMoveDirection = Vector3.zero;
    private float currentStamina;
    private float rotationY;
    private bool isHoldingRun = false; // Tracks if the button is held
    private bool isRunning = false; // Tracks if the player is running
    private Animator animator;

    public Transform animationTarget; // Drag and drop the child object with the Animator in the inspector

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = animationTarget.GetComponent<Animator>(); // Assign animator from child object
        currentStamina = staminaMax; // Initialize stamina to full
        rotationY = transform.eulerAngles.y;

        // Ensure Rigidbody is set up correctly
        rb.freezeRotation = true; // Prevent rotation from physics

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

        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        moveInput = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * moveInput;

        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            isRunning = isHoldingRun && currentStamina > 0;
            lastMoveDirection = moveInput.normalized * (isRunning ? runSpeed : normalSpeed);
        }
        else
        {
            lastMoveDirection = Vector3.zero;
        }

        // 🔥 Instantly switch to the correct animation
        if (animator != null)
        {
            if (isMoving)
            {
                animator.Play("Walk");  // 🔥 Instantly play Walk animation
            }
            else
            {
                animator.Play("Stand"); // 🔥 Instantly play Stand animation
            }
        }

        if (isRunning)
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
        }
        else if (!isHoldingRun && currentStamina < staminaMax)
        {
            currentStamina += staminaRechargeRate * Time.deltaTime;
            if (currentStamina > staminaMax) currentStamina = staminaMax;
        }

        staminaBar.fillAmount = currentStamina / staminaMax;

        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(lastMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // Apply movement using Rigidbody
        rb.velocity = new Vector3(lastMoveDirection.x, rb.velocity.y, lastMoveDirection.z);
    }
}
