using UnityEngine;
using TMPro;
using System.Net;
using System;
using UnityEngine.SceneManagement;

public class HostMenuManager : MonoBehaviour
{
    public TextMeshProUGUI ipText;

    private void Start()
    {
        SceneManager.LoadScene("facade_park", LoadSceneMode.Additive);
    }
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

    void Update()
    {
        ipText.text = GetLocalIPAddress();
    }
}
