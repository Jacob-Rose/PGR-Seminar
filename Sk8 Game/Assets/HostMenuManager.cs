using UnityEngine;
using TMPro;
using System.Net;
using System;

public class HostMenuManager : MonoBehaviour
{
    public TextMeshProUGUI ipText;
    public string GetLocalIPAddress()
    {
        IPHostEntry Host = default(IPHostEntry);
        string Hostname = null;
        Hostname = System.Environment.MachineName;
        Host = Dns.GetHostEntry(Hostname);
        string ip = "";
        foreach (IPAddress IP in Host.AddressList)
        {
            if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ip = Convert.ToString(IP);
            }
        }
        return ip;
    }

    public string GetRemoteIPAddress()
    {
        /*
        IPHostEntry Host = default(IPHostEntry);
        string Hostname = null;
        Hostname = System.Environment.MachineName;
        Host = Dns.GetHostEntry(Hostname);
        string ip = "";
        foreach (IPAddress IP in Host.AddressList)
        {
            if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ip = Convert.ToString(IP);
            }
        }
        return ip;
        */
        return "";
    }

    void Update()
    {
        ipText.text = GetLocalIPAddress();
    }
}
