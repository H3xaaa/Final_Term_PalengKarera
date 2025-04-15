using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class CameraController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Camera Settings")]
    public Transform target; // The player
    public float sensitivity = 0.2f;
    public Vector2 rotationLimits = new Vector2(-30f, 60f);
    public float distanceFromTarget = 5f;
    public float heightOffset = 2f;
    public float rotationSmoothing = 10f;

    [Header("UI Settings")]
    public GameObject touchPanel;

    private Vector2 rotation = Vector2.zero;
    private bool dragging = false;

    void Start()
    {
        if (touchPanel == null)
        {
            touchPanel = GameObject.Find("TouchPanel");
        }

        // Uncomment this if you prefer to assign events by code:
        //AssignEventTriggers(touchPanel);

        FindLocalPlayer();
    }

    void FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Ensure your player has the "Player" tag

        foreach (GameObject player in players)
        {
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                SetTarget(player.transform);
                Debug.Log("Camera assigned to: " + player.name);
                return;
            }
        }

        Debug.LogError("Local player not found!");
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null) return;

        target = newTarget;
        rotation.y = target.eulerAngles.y; // Align with player's direction
    }

    // -- Inspector-Friendly Wrapper Methods Using BaseEventData --
    public void HandlePointerDown(BaseEventData data)
    {
        OnPointerDown((PointerEventData)data);
    }

    public void HandlePointerUp(BaseEventData data)
    {
        OnPointerUp((PointerEventData)data);
    }

    public void HandleDrag(BaseEventData data)
    {
        OnDrag((PointerEventData)data);
    }

    // -- Interface Methods (Not visible in Inspector) --
    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || target == null) return;

        Vector2 dragDelta = eventData.delta;
        rotation.x -= dragDelta.y * sensitivity;
        rotation.y += dragDelta.x * sensitivity;

        rotation.x = Mathf.Clamp(rotation.x, rotationLimits.x, rotationLimits.y);
    }

    // -- (Optional) Code to Assign Event Triggers Programmatically --
    void AssignEventTriggers(GameObject panel)
    {
        EventTrigger eventTrigger = panel.GetComponent<EventTrigger>() ?? panel.AddComponent<EventTrigger>();

        if (eventTrigger.triggers == null)
        {
            eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
        }
        else
        {
            eventTrigger.triggers.Clear();
        }

        AddEventTrigger(eventTrigger, EventTriggerType.PointerDown, HandlePointerDown);
        AddEventTrigger(eventTrigger, EventTriggerType.PointerUp, HandlePointerUp);
        AddEventTrigger(eventTrigger, EventTriggerType.Drag, HandleDrag);
    }

    void AddEventTrigger(EventTrigger eventTrigger, EventTriggerType type, System.Action<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action.Invoke(data));
        eventTrigger.triggers.Add(entry);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion targetRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        Vector3 desiredPosition = target.position - targetRotation * Vector3.forward * distanceFromTarget + Vector3.up * heightOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationSmoothing);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothing);
    }
}
