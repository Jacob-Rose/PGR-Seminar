using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectionMenu : MonoBehaviour
{
    public TextMeshProUGUI ipText;
    public TextMeshProUGUI usernameText;
    public Button submitButton;
    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(ConnectToIP);
    }

    // Update is called once per frame

    void ConnectToIP()
    {
        string ip = ipText.text;
        VOnlinePlayer.m_Instance.ConnectToIP(ip.Trim(new char[] { (char)8203 }));
    }
}
