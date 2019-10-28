﻿using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Sockets;

public class VHostBehavior : Networked
{
    protected uint m_ListenSocket;
    public static VHostBehavior m_Instance;
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
                    m_Connections.Add(info.connection, null);
                    m_Server.SendMessageToConnection(info.connection, new PlayerConnectedMessage(GameManager.Instance.m_PlayerUsername).toBuffer(), SendType.Reliable);
                    break;

                case ConnectionState.ClosedByPeer:
                    m_Server.CloseConnection(info.connection);
                    Debug.Log("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                    GameManager.Instance.RemovePlayer(m_Connections[info.connection]);
                    m_Connections.Remove(info.connection);
                    break;
            }
        };
        base.Start();
    }


    public override void Update()
    {
        if (m_Server != null && m_Status != null)
        {
            m_Server.DispatchCallback(m_Status);
            PlayerUpdateMessage cPlayerUpdateMsg = new PlayerUpdateMessage(FindObjectOfType<ClientPlayer>().playerInfo, "host");
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

    public void SendMessageToAllPlayers(Message msg)
    {
        foreach (var pair in m_Connections)
        {
            m_Server.SendMessageToConnection(pair.Key, msg.toBuffer());
        }
    }

    protected override void HandleNetworkMessage(Message msg)
    {
        if(msg is PlayerConnectedMessage)
        {
            PlayerConnectedMessage nMsg = msg as PlayerConnectedMessage;
            m_NewConnectionName = new KeyValuePair<uint, string>(m_NetworkMessageConnectionSource, nMsg.playerID);
            GameManager.Instance.AddPlayer(nMsg.playerID);
        }
        if (msg is PlayerUpdateMessage)
        {
            PlayerUpdateMessage nMsg = msg as PlayerUpdateMessage;
            GameManager.Instance.UpdatePlayerInformation(nMsg.info, nMsg.playerID);
        }

        SendMessageToAllPlayers(msg);//all messages relayed
    }
}
