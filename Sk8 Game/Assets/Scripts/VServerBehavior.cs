using System.Collections;
using System.Collections.Generic;
using Valve.Sockets;
using UnityEngine;

/*
 * public class VServerBehavior : MonoBehaviour
{
    private NetworkingSockets m_Server = new NetworkingSockets();
    private Address m_Address = new Address();
    private uint m_ListenSocket;
    public ushort port = 9000;

   

    public void Awake()
    {
        m_Address.SetAddress("::0", port);
        m_ListenSocket = m_Server.CreateListenSocket(ref m_Address);
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

                case ConnectionState.Connecting:
                    m_Server.AcceptConnection(info.connection);
                    break;

                case ConnectionState.Connected:
                    Debug.Log("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                    break;

                case ConnectionState.ClosedByPeer:
                    m_Server.CloseConnection(info.connection);
                    Debug.Log("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                    break;
            }
        };

        const int maxMessages = 20;

        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
        int netMessagesCount = m_Server.ReceiveMessagesOnListenSocket(m_ListenSocket, netMessages, maxMessages);

        if (netMessagesCount > 0)
        {
            for (int i = 0; i < netMessagesCount; i++)
            {
                ref NetworkingMessage netMessage = ref netMessages[i];

                Debug.Log("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);

                
                netMessage.Destroy();
            }
        }
    }
}
*/