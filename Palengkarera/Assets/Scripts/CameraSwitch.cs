using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    public Button switchButton;
    private bool isFirstPerson = true;

    // Start is called before the first frame update
    void Start()
    {
        firstPersonCamera.enabled = true;
        thirdPersonCamera.enabled = false;

        switchButton.onClick.AddListener(SwitchCamera);
    }

    void SwitchCamera()
    {
        isFirstPerson = !isFirstPerson;
        firstPersonCamera.enabled = isFirstPerson;
        thirdPersonCamera.enabled = !isFirstPerson;
    }
}
