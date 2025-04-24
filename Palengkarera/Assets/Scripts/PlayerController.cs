using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
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
    public Transform animationTarget;

    private PhotonView photonView;

    private Trait assignedTrait;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            AssignUIElements();
            AssignCamera();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
            enabled = false;
            return;
        }

        Transform characterTransform = transform.Find("Character");
        if (characterTransform)
        {
            animationTarget = characterTransform;
            animator = characterTransform.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Character child not found! Ensure your prefab structure is correct.");
        }

        currentStamina = staminaMax;
        rb.freezeRotation = true;
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

            CameraController cameraController = mainCamera.GetComponent<CameraController>();
            if (cameraController)
            {
                cameraController.SetTarget(transform);
            }
            else
            {
                mainCamera.transform.SetParent(transform);
                mainCamera.transform.localPosition = new Vector3(0, 2, -4);
            }
        }
        else
        {
            Debug.LogError("MainCamera not found! Ensure it has the correct tag.");
        }
    }

    // Buff System
    private List<Buff> activeBuffs = new List<Buff>();
    public void ApplyBuff(Buff buff)
    {
        if (!activeBuffs.Contains(buff))
        {
            StartCoroutine(HandleBuff(buff));
        }
    }

    private IEnumerator HandleBuff(Buff buff)
    {
        activeBuffs.Add(buff);
        buff.Apply(this);

        yield return new WaitForSeconds(buff.duration);

        buff.Remove(this);
        activeBuffs.Remove(buff);
    }

    // Traits
    void Start()
    {
        AssignRandomTrait();
        ApplyTraitEffects();
    }

    void AssignRandomTrait()
    {
        Debug.Log($"Traits Count: {TraitSystem.inGameTrait.Count}");

        int randomIndex = Random.Range(0, TraitSystem.inGameTrait.Count);
        assignedTrait = TraitSystem.inGameTrait[randomIndex];

        if (assignedTrait != null)
        {
            Debug.Log($"Assigned Trait: {assignedTrait.Name}");
        }
        else
        {
            Debug.LogError("Trait Assignment Failed!");
        }
    }

    void ApplyTraitEffects()
    {
        if (assignedTrait == null) return;

        if (assignedTrait.Name == "Athletic")
        {
            float oldSpeed = normalSpeed;
            normalSpeed = oldSpeed + 3f;
            Debug.Log($"Movement Speed Increased: From {oldSpeed} to {normalSpeed}");
        }
        else if (assignedTrait.Name == "Locked In")
        {
            float oldStaminaDepletionRate = staminaDepletionRate;
            staminaDepletionRate = oldStaminaDepletionRate - 10f;
            Debug.Log($"Stamina Depletion Reduced: From {oldStaminaDepletionRate} to {staminaDepletionRate}");
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public Trait GetTrait() => assignedTrait;

    public bool HasTrait(string traitName) =>
        assignedTrait != null && assignedTrait.Name == traitName;

    void Update()
    {
        if (!photonView.IsMine || joystick == null || cameraTransform == null) return;

        // Input from joystick and keyboard
        float horizontal = joystick.Horizontal + (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float vertical = joystick.Vertical + (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);

        Vector2 inputVector = new Vector2(horizontal, vertical);
        if (inputVector.magnitude < 0.1f) return;

        // Camera-relative movement fix
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveInput = camForward * inputVector.y + camRight * inputVector.x;

        bool isMoving = moveInput.magnitude > 0.1f;
        bool isHoldingRun = isHoldingRunButton || Input.GetKey(KeyCode.LeftShift);

        float speed = isHoldingRun && currentStamina > 0 ? runSpeed : normalSpeed;
        Vector3 moveVelocity = moveInput.normalized * speed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;

        // Animation
        if (animator != null)
        {
            animator.Play(isMoving ? "Walk" : "Stand");
        }

        // Stamina
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

        // Smooth rotation
        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetJoystick(MobileJoystick assignedJoystick)
    {
        joystick = assignedJoystick;
    }
}