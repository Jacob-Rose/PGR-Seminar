using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class HostMenuManager : MonoBehaviour
{
    public ServerBehavior m_Server;

    public TMPro.TextMeshProUGUI ipText;

    public TMPro.TextMeshProUGUI player1Text;

    // Start is called before the first frame update
    void Start()
    {
        ipText.text = LocalIPAddress();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}
