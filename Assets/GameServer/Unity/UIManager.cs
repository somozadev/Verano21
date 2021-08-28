using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }
    public GameObject startMenu;
    public InputField usernameField;


    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false; 
        Client.instance.ConnectToServer();
    }

}
