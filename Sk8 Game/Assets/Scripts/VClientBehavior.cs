using System.Collections;
using System.Collections.Generic;
using Valve.Sockets;
using UnityEngine;

/*
public class VClientBehavior : MonoBehaviour
{
    NetworkingSockets m_Client = new NetworkingSockets();
    Address m_Address = new Address();
    public ushort port = 9000;
    uint m_Connection;

    void Awake()
    {
        
        m_Address.SetAddress("::1", port);
    }

    void OnDestroy()
    {
        //DeinitializeValveSockets();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        StatusCallback status = (info, context) => {
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    break;

                case ConnectionState.Connected:
                    Debug.Log("Client connected to server - ID: " + m_Connection);
                    break;

                case ConnectionState.ClosedByPeer:
                    m_Client.CloseConnection(m_Connection);
                    Debug.Log("Client disconnected from server");
                    break;

                case ConnectionState.ProblemDetectedLocally:
                    m_Client.CloseConnection(m_Connection);
                    Debug.Log("Client unable to connect");
                    break;
            }
        };
        const int maxMessages = 20;

        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

        m_Client.DispatchCallback(status);


        int netMessagesCount = m_Client.ReceiveMessagesOnConnection(m_Connection, netMessages, maxMessages);

        if (netMessagesCount > 0)
        {
            for (int i = 0; i < netMessagesCount; i++)
            {
                ref NetworkingMessage netMessage = ref netMessages[i];

                Debug.Log("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);

                netMessage.Destroy();
            }
        }
        
    }

    public void ConnectToIP(string ip)
    {
        m_Address.SetAddress(ip, port);
        m_Connection = m_Client.Connect(ref m_Address);
    }

    public static void InitializeValveSockets()
    {
        Valve.Sockets.Library.Initialize();
    }

    public static void DeinitializeValveSockets()
    {
        Valve.Sockets.Library.Deinitialize();
    }
}
*/