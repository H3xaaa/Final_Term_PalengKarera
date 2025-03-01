using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private MobileJoystick joystick;
    private Transform cameraTransform;
    private Image staminaBar;
    private Button runButton;

    public float normalSpeed = 5f;
    public float runSpeed = 15f;
    public float rotationSpeed = 10f;

    public float staminaMax = 100f;
    public float staminaDepletionRate = 25f;
    public float staminaRechargeRate = 25f;

    private Rigidbody rb;
    private float currentStamina;
    private bool isHoldingRunButton = false;
    private Animator animator;
    public Transform animationTarget; // Player model with Animator

    private PhotonView photonView;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        if (animationTarget == null)
        {
            animationTarget = GetComponentInChildren<Animator>()?.transform;
            if (animationTarget == null)
                Debug.LogError("No animationTarget found! Assign it manually in the Inspector.");
        }

        animator = animationTarget?.GetComponent<Animator>();

        currentStamina = staminaMax;
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            AssignUIElements();
            AssignCamera();
        }
        else
        {
            enabled = false;
        }
    }

    void AssignUIElements()
    {
        GameObject uiCanvas = GameObject.Find("UI_Canvas");

        if (uiCanvas)
        {
            Transform inGamePanel = uiCanvas.transform.Find("In-Game");
            if (inGamePanel)
            {
                joystick = inGamePanel.transform.Find("Joystick")?.GetComponent<MobileJoystick>();
            }

            staminaBar = uiCanvas.transform.Find("StaminaBar")?.GetComponent<Image>();
            runButton = uiCanvas.transform.Find("RightSideButtons/RunButton")?.GetComponent<Button>();

            if (runButton)
            {
                EventTrigger trigger = runButton.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry pressEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                pressEntry.callback.AddListener((data) => isHoldingRunButton = true);
                trigger.triggers.Add(pressEntry);

                EventTrigger.Entry releaseEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                releaseEntry.callback.AddListener((data) => isHoldingRunButton = false);
                trigger.triggers.Add(releaseEntry);
            }
        }
        else
        {
            Debug.LogError("UI_Canvas not found! Make sure it's in the scene.");
        }
    }

    void AssignCamera()
    {
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        if (mainCamera)
        {
            cameraTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogError("MainCamera not found! Ensure the camera exists and has the correct tag.");
        }
    }

    void Update()
    {
        if (!photonView.IsMine || joystick == null || cameraTransform == null) return;

        float horizontal = joystick.Horizontal + (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float vertical = joystick.Vertical + (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);

        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        moveInput = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * moveInput;

        bool isMoving = moveInput.magnitude > 0.1f;
        bool isHoldingRun = isHoldingRunButton || Input.GetKey(KeyCode.LeftShift);

        float speed = isHoldingRun && currentStamina > 0 ? runSpeed : normalSpeed;

        Vector3 moveVelocity = moveInput.normalized * speed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;

        // **Updated Animation Logic**
        if (animator != null)
        {
            if (isMoving)
            {
                animator.Play("Walk"); // Play Walk animation immediately when moving
            }
            else
            {
                animator.Play("Stand"); // Play Stand animation immediately when stopping
            }
        }

        // Handle stamina
        if (isHoldingRun)
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
        }
        else if (currentStamina < staminaMax)
        {
            currentStamina += staminaRechargeRate * Time.deltaTime;
        }

        if (staminaBar)
        {
            staminaBar.fillAmount = currentStamina / staminaMax;
        }

        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
