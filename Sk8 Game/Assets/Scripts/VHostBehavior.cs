using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Sockets;

public class VHostBehavior : Networked
{
    private StatusCallback m_Status;
    protected ConnectionManager m_Connections = new ConnectionManager();

    public static VHostBehavior m_Instance;

    public override void Start()
    {
        m_Instance = this;
        base.Start();
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
                //todo check if this is the current player
                ConnectionInfo cInfo = m_Instance.m_Connections.addConnection(info.connection);
                m_Instance.HandleNetworkMessage(new PlayerConnectedMessage(cInfo.player.playerID));
                break;
            case ConnectionState.ClosedByPeer:
                m_Instance.m_Server.CloseConnection(info.connection);
                m_Instance.m_Connections.removeConnection(info.connection);
                Debug.Log("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                break;
        }
    }

    protected override void HandleNetworkMessage(Message msg)
    {
        if(msg is PlayerConnectedMessage)
        {
            PlayerConnectedMessage nMsg = msg as PlayerConnectedMessage;
            for(int i =0; i < m_Connections.getConnectionCount(); i++)
            {
                PlayerConnectedMessage playerToSend = new PlayerConnectedMessage(m_Connections.getConnection(i).player.playerID);
                m_Server.SendMessageToConnection(m_Connections.getConnectionFromPlayerID(nMsg.playerID), playerToSend.toBuffer());
                PlayerUpdateMessage infoToSend = new PlayerUpdateMessage(m_Connections.getConnection(i).player.playerInfo);
                m_Server.SendMessageToConnection(m_Connections.getConnectionFromPlayerID(nMsg.playerID), infoToSend.toBuffer());
            }
            
            SendPlayerConnectedMessage((msg as PlayerConnectedMessage).playerID);
        }
    }

    public void SendPlayerConnectedMessage(int playerID)
    {
        PlayerConnectedMessage msg = new PlayerConnectedMessage(playerID);
        for (int i = 0; i < m_Connections.getConnectionCount(); i++)
        {
            if (m_Connections.getConnection(i).player.playerID != playerID)
            {
                m_Server.SendMessageToConnection(m_Connections.getConnection(i).connection, msg.toBuffer());
            }
        }
    }

    public void SendStartGameMessage()
    {
        DateTime timeToStart = DateTime.Now + TimeSpan.FromSeconds(4.0f);
        GameStartMessage msg = new GameStartMessage(timeToStart.Ticks);
        SendMessageToAllPlayers(msg);
    }

    public void SendMessageToAllPlayers(Message msg)
    {
        for(int i = 0; i < m_Connections.getConnectionCount(); i++)
        {
            m_Server.SendMessageToConnection(m_Connections.getConnection(i).connection, msg.toBuffer());
        }
    }

    public override void Update()
    {
        if(m_Server != null)
        {
            m_Server.DispatchCallback(m_Status);
        }
        base.Update();
        
    }
}
