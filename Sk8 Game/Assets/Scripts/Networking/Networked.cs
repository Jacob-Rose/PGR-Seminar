using AOT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Sockets;

public abstract class Networked : MonoBehaviour
{

    protected NetworkingSockets m_Server = new NetworkingSockets();
    protected Address m_Address = new Address();
    protected uint m_ListenSocket;
    protected StatusCallback m_Status;
    public ushort m_Port = 9000;

    private const int maxMessages = 20;
    private NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

    byte[] messageDataBuffer = new byte[256];

    public ClientPlayer m_ClientPlayer; //all networked will have a single client player that sends data

    protected abstract void HandleNetworkMessage(Message msg);

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
        SpawnClientPlayer();
    }

    public void SpawnClientPlayer()
    {
        m_ClientPlayer = ((GameObject)Instantiate(Resources.Load("Prefabs/ClientPlayer"))).GetComponent<ClientPlayer>();
    }

    public virtual void Update()
    {
        if (m_Server != null && m_Status != null)
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
                    Message m = Message.decipherMessage(messageDataBuffer);

                    HandleNetworkMessage(m);
                }
            }
        }
    }

    public void StartGame()
    {
        Toolbox.Instance.StartGame();
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
