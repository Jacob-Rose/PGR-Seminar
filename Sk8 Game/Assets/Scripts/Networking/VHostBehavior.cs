using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Sockets;

public class VHostBehavior : Networked
{
    protected uint m_ListenSocket;
    public static VHostBehavior Instance { get { return m_Instance; } }
    private static VHostBehavior m_Instance;
    public Dictionary<uint, string> m_Connections = new Dictionary<uint, string>();
    private KeyValuePair<uint, string>? m_NewConnectionName = null;
    public uint m_NetworkMessageConnectionSource = 0; //where the message came from

    public override void OnDestroy()
    {
        foreach(var i in m_Connections) //close all connections
        {
            m_Server.CloseConnection(i.Key);
        }
        base.OnDestroy();
    }

    public override void Start()
    {
        m_Instance = this;
        m_Address.SetAddress("::0", m_Port);
        m_ListenSocket = m_Server.CreateListenSocket(ref m_Address);
        SetupStatusDelegate();
        base.Start();
    }

    public void SetupStatusDelegate()
    {
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
                    PlayerConnected(info.connection);
                    break;
                case ConnectionState.ClosedByPeer:
                    m_Server.CloseConnection(info.connection);
                    Debug.Log("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                    PlayerDisconnected(info.connection, m_Connections[info.connection]);
                    break;
            }
        };
    }

    public void PlayerConnected(uint connection)
    {
        m_Connections.Add(connection, null);
        m_Server.SendMessageToConnection(connection, new PlayerConnectedMessage(GameManager.Instance.m_PlayerUsername).toBuffer(), SendType.Reliable);
        foreach(var pair in m_Connections)
        {
            if(pair.Key != connection)
            {
                m_Server.SendMessageToConnection(connection, new PlayerConnectedMessage(pair.Value).toBuffer(), SendType.Reliable);
            }
        }
    }

    public void PlayerDisconnected(uint connection, string playerID)
    {
        PlayerDisconnectedMessage dMsg = new PlayerDisconnectedMessage(playerID);
        SendMessageToAllPlayers(dMsg, SendType.Reliable);
        GameManager.Instance.RemovePlayer(playerID);
        m_Connections.Remove(connection);
    }

    public override void Update()
    {
        if (m_Server != null && m_Status != null)
        {
            m_Server.DispatchCallback(m_Status); //check for new or changed connections
            ClientPlayer cPlayer = GameManager.Instance.ClientPlayer;
            if (cPlayer != null)
            {
                PlayerUpdateMessage cPlayerUpdateMsg = new PlayerUpdateMessage(FindObjectOfType<ClientPlayer>().playerInfo, GameManager.Instance.m_PlayerUsername);
                SendMessageToAllPlayers(cPlayerUpdateMsg);
            }
            
            foreach (var c in m_Connections)
            {
                netMessageCount = m_Server.ReceiveMessagesOnConnection(c.Key, netMessages, maxMessages);
                m_NetworkMessageConnectionSource = c.Key;
                readNetworkMessages();
            }
            if(m_NewConnectionName.HasValue)
            {
                m_Connections[m_NewConnectionName.Value.Key] = m_NewConnectionName.Value.Value;
            }
            
        }
    }
    public void StartGameInSeconds(float seconds)
    {
        DateTime timeToStart = DateTime.Now.AddSeconds(seconds);
        GameManager.Instance.StartGameInSeconds(seconds);
        GameStartMessage msg = new GameStartMessage(timeToStart.Ticks);
        SendMessageToAllPlayers(msg);
    }

    public void SendMessageToAllExceptPlayer(string player, Message m, SendType type = SendType.NoDelay)
    {
        foreach (var pair in m_Connections)
        {
            if(pair.Value != player && pair.Value != null)
            {
                m_Server.SendMessageToConnection(pair.Key, m.toBuffer(), type);
            }
        }
    }

    public void SendMessageToAllPlayers(Message msg, SendType type = SendType.NoDelay)
    {
        foreach (var pair in m_Connections)
        {
            m_Server.SendMessageToConnection(pair.Key, msg.toBuffer(), type);
        }
    }

    protected override void HandleNetworkMessage(Message msg)
    {
        if(msg is PlayerConnectedMessage)
        {
            PlayerConnectedMessage nMsg = msg as PlayerConnectedMessage;
            m_NewConnectionName = new KeyValuePair<uint, string>(m_NetworkMessageConnectionSource, nMsg.playerID);
            GameManager.Instance.AddPlayer(nMsg.playerID);
            SendMessageToAllExceptPlayer(nMsg.playerID, msg);
        }
        if (msg is PlayerUpdateMessage)
        {
            PlayerUpdateMessage nMsg = msg as PlayerUpdateMessage;
            GameManager.Instance.UpdatePlayerInformation(ref nMsg.info, nMsg.playerID);
            SendMessageToAllExceptPlayer(nMsg.playerID, nMsg);
        }
        if(msg is ObstacleModifiedMessage)
        {
            ObstacleModifiedMessage nMsg = msg as ObstacleModifiedMessage;
            Obstacle.m_AllObstacles[(int)nMsg.obstacleID].InteractedWith(GameManager.Instance.GetPlayer(nMsg.playerID));
            SendMessageToAllExceptPlayer(nMsg.playerID, nMsg);
        }
    }

    
}
