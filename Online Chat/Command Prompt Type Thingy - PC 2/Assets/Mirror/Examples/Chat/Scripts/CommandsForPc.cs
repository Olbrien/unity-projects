using Mirror.Examples.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandsForPc : MonoBehaviour
{
    ChatNetworkManager chatNetworkManager;

    public InputField thisInputField;

    void Start()
    {
        chatNetworkManager = FindObjectOfType<ChatNetworkManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            chatNetworkManager.StartHost();
            gameObject.SetActive(false);
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            thisInputField.ActivateInputField();
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            chatNetworkManager.StartClient();
            gameObject.SetActive(false);
        }
    }
}
