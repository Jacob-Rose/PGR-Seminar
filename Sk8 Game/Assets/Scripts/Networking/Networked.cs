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
    public ushort m_Port = 9000;

    private const int maxMessages = 20;
    private NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

    byte[] messageDataBuffer = new byte[256];

    public ClientPlayer m_ClientPlayer; //all networked will have a single client player that sends data

    protected abstract void HandleNetworkMessage(Message msg);

    public virtual void Update()
    {
        if (m_Server != null)
        {
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

    public static void InitializeValveSockets()
    {
        Valve.Sockets.Library.Initialize();
    }

    public static void DeinitializeValveSockets()
    {
        Valve.Sockets.Library.Deinitialize();
    }
}
