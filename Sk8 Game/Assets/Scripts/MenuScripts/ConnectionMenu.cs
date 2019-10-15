using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectionMenu : MonoBehaviour
{
    public TextMeshProUGUI ipText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConnectToIP()
    {
        VOnlinePlayer.m_Instance.ConnectToIP(ipText.text);
    }
}
