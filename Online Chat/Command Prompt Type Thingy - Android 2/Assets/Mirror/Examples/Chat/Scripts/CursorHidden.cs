using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHidden : MonoBehaviour
{
    public bool android;

    void Start()
    {

        if (android)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}
