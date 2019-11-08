using AOT;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using Valve.Sockets;

public abstract class Networked : MonoBehaviour
{
    protected NetworkingSockets m_Server = new NetworkingSockets();

    protected Address m_Address = new Address();
    protected StatusCallback m_Status;
    public ushort m_Port = 9000;

    protected const int maxMessages = 20;
    protected int netMessageCount = 0;
    protected NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

    byte[] messageDataBuffer = new byte[256];
    

    public virtual void Awake()
    {
        InitializeValveSockets();
    }

    public virtual void OnDestroy()
    {
        DeinitializeValveSockets();
    }

    public virtual void Start()
    {
        DontDestroyOnLoad(this);
    }

    public virtual void Update()
    {
        if (m_Server != null && m_Status != null)
        {
            m_Server.DispatchCallback(m_Status);
        }
    }

    public void readNetworkMessages()
    {
        if (netMessageCount > 0)
        {
            for (int i = 0; i < netMessageCount; i++)
            {
                ref NetworkingMessage netMessage = ref netMessages[i];
                netMessage.CopyTo(messageDataBuffer);
                uint connection = netMessage.connection; //who sent it
                netMessage.Destroy();
                Message m = Message.decipherMessage(messageDataBuffer);
                HandleNetworkMessage(m);
            }
        }
    }

    protected virtual void HandleNetworkMessage(Message msg)
    {
        if (msg is PlayerConnectedMessage) //reposition players for both
        {
            List<Player> players = GameManager.Instance.m_Players;
        }
    }

    public static IPAddress GetIP()
    {
        var myClient = new WebClient();
        string myExtIP = myClient.DownloadString("http://checkip.dyndns.org");
        myExtIP = myExtIP.Substring(myExtIP.IndexOf(": ") + 2);
        myExtIP = myExtIP.Substring(0, myExtIP.IndexOf("<"));
        char[] charIp = myExtIP.ToCharArray();
        IPAddress address = IPAddress.Parse(myExtIP);
        return address;
    }


    public static void InitializeValveSockets()
    {
        Library.Initialize();
    }

    public static void DeinitializeValveSockets()
    {
        Library.Deinitialize();
    }
}
