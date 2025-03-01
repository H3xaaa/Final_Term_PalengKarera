using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Camera Settings")]
    public Transform target; // Assign player object in Inspector
    public float sensitivity = 0.2f;
    public Vector2 rotationLimits = new Vector2(-30f, 60f);
    public float distanceFromTarget = 5f;
    public float heightOffset = 2f;

    [Header("UI Settings")]
    public GameObject touchPanel; // Assign manually or automatically finds it

    private Vector2 rotation = Vector2.zero;
    private bool dragging = false;

    void Start()
    {
        if (touchPanel == null)
        {
            touchPanel = GameObject.Find("TouchPanel");
        }

        if (touchPanel)
        {
            AssignEventTriggers(touchPanel);
        }
        else
        {
            Debug.LogError("TouchPanel not found! Make sure it's active in the scene.");
        }
    }

    public void SetTarget(Transform newTarget) // 🔥 Added back this function
    {
        target = newTarget;
        rotation.y = target.eulerAngles.y; // Align with player's direction
    }

    void AssignEventTriggers(GameObject panel)
    {
        EventTrigger eventTrigger = panel.GetComponent<EventTrigger>() ?? panel.AddComponent<EventTrigger>();

        eventTrigger.triggers.Clear(); // Clear existing triggers

        AddEventTrigger(eventTrigger, EventTriggerType.PointerDown, OnPointerDown);
        AddEventTrigger(eventTrigger, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTrigger(eventTrigger, EventTriggerType.Drag, OnDrag);
    }

    void AddEventTrigger(EventTrigger eventTrigger, EventTriggerType type, System.Action<PointerEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action.Invoke((PointerEventData)data));
        eventTrigger.triggers.Add(entry);
    }

    public void OnPointerDown(PointerEventData eventData) => dragging = true;
    public void OnPointerUp(PointerEventData eventData) => dragging = false;

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || target == null) return;

        Vector2 dragDelta = eventData.delta;
        rotation.x -= dragDelta.y * sensitivity;
        rotation.y += dragDelta.x * sensitivity;

        rotation.x = Mathf.Clamp(rotation.x, rotationLimits.x, rotationLimits.y);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion camRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        transform.position = target.position - camRotation * Vector3.forward * distanceFromTarget + Vector3.up * heightOffset;
        transform.rotation = camRotation;
    }
}
