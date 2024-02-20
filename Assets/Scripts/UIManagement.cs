using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagement : MonoBehaviour
{
    public TMP_InputField userName;
    //public TMP_InputField userAge;

    public Button startButton;
    public Button quitButton;

    public string uname;
    public int uage;

    public static UIManagement instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void GetUserDetails()
    {
        uname = userName.text;
        //uage = int.Parse(userAge.text);

        Debug.Log("Name: "+ uname);

        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            GetUserDetails();
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
            Debug.Log("Application Quit");
        });
    }

}
