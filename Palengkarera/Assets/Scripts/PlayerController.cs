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
    public Transform animationTarget; // Player model with Animator

    private PhotonView photonView;

    private Trait assignedTrait; // for Trait Assigning 

    // ✅ Called when the player is instantiated in the network
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        photonView = GetComponent<PhotonView>(); // Ensure PhotonView is assigned

        if (photonView.IsMine)
        {
            AssignUIElements();
            AssignCamera();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>(); // ✅ Fix: Assign PhotonView in Awake

        if (!photonView.IsMine) // ✅ Only control the local player
        {
            rb.isKinematic = true; // Disable physics for non-local players
            enabled = false;
            return;
        }

        // ✅ Ensure we get the correct Animator from the "Character" child
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

    // ✅ Assigns the camera to follow this player
    void AssignCamera()
    {
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");

        if (mainCamera)
        {
            CameraController cameraController = mainCamera.GetComponent<CameraController>();
            if (cameraController)
            {
                cameraController.SetTarget(transform); // ✅ Camera follows the player
            }
            else
            {
                mainCamera.transform.SetParent(transform); // Attach camera to player as fallback
                mainCamera.transform.localPosition = new Vector3(0, 2, -4);
            }
        }
        else
        {
            Debug.LogError("MainCamera not found! Ensure it has the correct tag.");
        }
    }

    //Buff and Debuff
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


    //Assign Random Trait
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

    public Trait GetTrait()
    {
        return assignedTrait;
    }

    public bool HasTrait(string traitName)
    {
        return assignedTrait != null && assignedTrait.Name == traitName; 
    }


    void Update()
    {
        if (!photonView.IsMine || joystick == null) return;

        float horizontal = joystick.Horizontal + (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float vertical = joystick.Vertical + (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);

        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        moveInput = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * moveInput;

        bool isMoving = moveInput.magnitude > 0.1f;
        bool isHoldingRun = isHoldingRunButton || Input.GetKey(KeyCode.LeftShift);

        float speed = isHoldingRun && currentStamina > 0 ? runSpeed : normalSpeed;

        Vector3 moveVelocity = moveInput.normalized * speed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;

        // ✅ **Animation Handling**
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

        // ✅ **Handle stamina**
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

        // ✅ **Smooth Rotation**
        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ✅ Restored to fix errors in Spawner and NetworkManager
    public void SetJoystick(MobileJoystick assignedJoystick)
    {
        joystick = assignedJoystick;
    }
}
