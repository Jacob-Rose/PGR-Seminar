using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Sockets;

public class VHostBehavior : Networked
{
    protected uint m_ListenSocket;
    public static VHostBehavior m_Instance;
    public Dictionary<uint, Address> m_Connections = new Dictionary<uint, Address>();

    public override void Start()
    {
        m_Instance = this;
        m_Address.SetAddress("::0", m_Port);
        m_ListenSocket = m_Server.CreateListenSocket(ref m_Address);

        m_Status = (info, context) => {
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    break;

                case ConnectionState.Connecting:
                    m_Server.AcceptConnection(info.connection);
                    break;

                case ConnectionState.Connected:
                    Debug.Log("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                    m_Connections.Add(info.connection, info.connectionInfo.address);
                    GameManager.Instance.AddPlayer(info.connectionInfo.address.GetIP());

                    break;

                case ConnectionState.ClosedByPeer:
                    m_Server.CloseConnection(info.connection);
                    Debug.Log("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                    m_Connections.Remove(info.connection);
                    GameManager.Instance.RemovePlayer(info.connectionInfo.address.GetIP());
                    break;
            }
        };
        base.Start();
    }
    public void AddNetworkedPlayer(uint connection, Address id)
    {

    }


    public override void Update()
    {
        if(m_Server != null && m_Status != null)
        {
            netMessageCount = m_Server.ReceiveMessagesOnListenSocket(m_ListenSocket, netMessages, maxMessages);
        }
        base.Update();
    }



    public void StartGameInSeconds(float seconds)
    {
        DateTime timeToStart = DateTime.Now.AddSeconds(seconds);
        GameManager.Instance.StartGameInSeconds(seconds);
        GameStartMessage msg = new GameStartMessage(timeToStart.Ticks);
        SendMessageToAllPlayers(msg);
    }

    public void SendPlayerConnectedMessage(Address playerWhoJoined)
    {
        PlayerConnectedMessage msg = new PlayerConnectedMessage(playerWhoJoined.GetIP());
        foreach (var pair in m_Connections)
        {
            if (pair.Value.GetIP() != playerWhoJoined.GetIP())
            {
                m_Server.SendMessageToConnection(pair.Key, msg.toBuffer());
            }
        }
    }

    public void SendMessageToAllPlayers(Message msg)
    {
        foreach (var pair in m_Connections)
        {
            m_Server.SendMessageToConnection(pair.Key, msg.toBuffer());
        }
    }

    protected override void HandleNetworkMessage(Message msg)
    {
        throw new NotImplementedException();
    }
}
