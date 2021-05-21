using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectInputToWrite : MonoBehaviour
{
    public GameObject usernameGO;
    public GameObject serverGO;

    public InputField thisInputField;

    public bool username;

    void Update()
    {
        thisInputField.ActivateInputField();

        if (Input.GetKeyDown(KeyCode.Return) && username == true)
        {
            serverGO.SetActive(true);
            usernameGO.SetActive(false);
        }
    }
}
