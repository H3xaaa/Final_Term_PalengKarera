using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibrate : MonoBehaviour
{
    public void VibrateButton()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#else
        Debug.Log("Vibration not supported on this platform.");
#endif
    }
}