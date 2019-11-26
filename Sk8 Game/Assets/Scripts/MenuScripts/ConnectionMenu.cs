using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectionMenu : MonoBehaviour
{
    public TextMeshProUGUI ipText;
    public TextMeshProUGUI usernameText;
    public Button submitButton;
    public Button mainMenuButton;

    public bool ConnectionAttempted = false;
    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(ButtonPressedConnect);
        mainMenuButton.onClick.AddListener(GameManager.Instance.ResetGame);
        SceneManager.LoadScene("facade_park", LoadSceneMode.Additive);
    }

    // Update is called once per frame

    void ConnectToIP()
    {
        string ip = ipText.text;
        VOnlinePlayer.Instance.ConnectToIP(ip.Trim(new char[] { (char)8203 }));
    }

    void TestIfConnectionSuccessful()
    {
        if(VOnlinePlayer.Instance.Connected)
        {
            submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Leave";
            submitButton.onClick.RemoveListener(ButtonPressedConnect);
            submitButton.onClick.AddListener(ButtonPressedDisconnect);
            ConnectionAttempted = true;
        }
        else
        {
            ConnectionAttempted = true;
        }
    }

    void ButtonPressedConnect()
    {
        ConnectToIP();
        Invoke("TestIfConnectionSuccessful", 0.2f);
    }

    void ButtonPressedDisconnect()
    {
        GameManager.Instance.ResetGame();
    }

    private void OnGUI()
    {
        
        if(ConnectionAttempted)
        {
            GUIStyle textStyle = GUI.skin.label;
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.fontSize = 24;
            GUI.color = new Color(0.9f, 0.5f, 0.5f);
            if (VOnlinePlayer.Instance.Connected)
            {
                GUI.Label(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.2f),
                    "Connected", textStyle);
            }
            else if (!VOnlinePlayer.Instance.Connected)
            {
                GUI.Label(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.2f),
                    "Connection Failed", textStyle);
            }
        }
        
    }
}
