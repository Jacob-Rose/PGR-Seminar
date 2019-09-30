using System.Collections;
using System.Collections.Generic;
using Valve.Sockets;
using UnityEngine;
using AOT;
using System;

/*Some Server Info and How To Use
 * https://unitylist.com/p/ljy/Unity-Valve-Game-Networking-Sockets-chat-example
 */
public class VServerBehavior : MonoBehaviour
{
    private NetworkingSockets m_Server = new NetworkingSockets();
    private Address m_Address = new Address();
    private StatusCallback m_Status;
    private ConnectionManager m_Connection;
    private uint m_ListenSocket;
    public ushort m_Port = 9000;

    private const int maxMessages = 20;
    private NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

    byte[] messageDataBuffer = new byte[256];

    static VServerBehavior m_Instance;

    public void Awake()
    {
        m_Instance = this;
        m_Address.SetAddress("::0", m_Port);
        m_ListenSocket = m_Server.CreateListenSocket(ref m_Address);

        m_Status = OnServerStatusUpdate;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [MonoPInvokeCallback(typeof(StatusCallback))]
    static void OnServerStatusUpdate(StatusInfo info, System.IntPtr context)
    {
        switch (info.connectionInfo.state)
        {
            case ConnectionState.None:
                break;

            case ConnectionState.Connecting:
                m_Instance.m_Server.AcceptConnection(info.connection);
                break;

            case ConnectionState.Connected:
                Debug.Log("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                m_Instance.m_Connection.addConnection(info.connection);
                break;

            case ConnectionState.ClosedByPeer:
                m_Instance.m_Server.CloseConnection(info.connection);
                m_Instance.m_Connection.removeConnection(info.connection);
                Debug.Log("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                break;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if(m_Server != null)
        {
            m_Server.DispatchCallback(m_Status);

            int netMessagesCount = m_Server.ReceiveMessagesOnListenSocket(m_ListenSocket, netMessages, maxMessages);

            if (netMessagesCount > 0)
            {
                for (int i = 0; i < netMessagesCount; i++)
                {
                    ref NetworkingMessage netMessage = ref netMessages[i];
                    Debug.Log("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                    netMessage.CopyTo(messageDataBuffer);
                    uint connection = netMessage.connection; //who sent it
                    netMessage.Destroy();
                    Message m = DecipherMessageBuffer();

                    HandleMessageBuffer(connection);
                }
            }
        }
    }

    Message DecipherMessageBuffer()
    {
        //DateTime timeToStart = DateTime.Now; //global time
        //timeToStart.AddSeconds(3.0f);//add three seconds to start
        //timeToStart.ToUniversalTime().Ticks;
    }

    void HandleMessageBuffer(uint connection)
    {
        //m_Server.SendMessageToConnection()
    }

    void StartGame()
    {
        
    }
}
