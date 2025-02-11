using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Transform target;
    public float sensitivity = 0.2f;
    public Vector2 rotationLimits = new Vector2(-30f, 60f);
    public float distanceFromTarget = 5f;
    public float heightOffset = 2f;

    private Vector2 rotation = new Vector2(0, 0);
    private bool dragging = false;

    void Start()
    {
        if (target != null)
        {
            rotation.y = target.eulerAngles.y;
        }
    }

    public void OnPointerDownWrapper()
    {
        dragging = true;
    }

    public void OnPointerUpWrapper()
    {
        dragging = false;
    }

    public void OnDragWrapper()
    {
        if (!dragging) return;

        Vector2 dragDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        rotation.x -= dragDelta.y * sensitivity;
        rotation.y += dragDelta.x * sensitivity;

        rotation.x = Mathf.Clamp(rotation.x, rotationLimits.x, rotationLimits.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownWrapper();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpWrapper();
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragWrapper();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion camRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        transform.position = target.position - camRotation * Vector3.forward * distanceFromTarget + Vector3.up * heightOffset;
        transform.rotation = camRotation;
    }

    public void TestFunction()
    {
        Debug.Log("Test function called");
    }

    void Awake()
    {
        Debug.Log("CameraController is Awake");
    }
}
