﻿using AOT;
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
    public uint m_NetworkMessageConnectionSource = 0; //where the message came from
    public KeyValuePair<uint, string>? m_ConnectionToAdd = null; //need to add after due to foreach on connections

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
    }

    public void PlayerDisconnected(uint connection, string playerID)
    {
        PlayerDisconnectedMessage dMsg = new PlayerDisconnectedMessage(playerID);
        SendMessageToAllPlayers(dMsg, SendType.Reliable);
        GameManager.Instance.RemovePlayer(playerID);
        m_Connections.Remove(connection);
        Invoke("RealignPlayersAndSend", 0.1f); //need a delay for them to add the player
    }

    //NOTE: CURRENTLY NO TESTING FOR ANY SECURITY, ONE PLAYER COULD SEND FAKE PACKETS IF THEY WERE SMART ENOUGH AND RIG THE GAME
    public override void Update()
    {
        if (m_Server != null && m_Status != null)
        {
            m_Server.DispatchCallback(m_Status); //check for new or changed connections
            foreach (var c in m_Connections)
            {
                netMessageCount = m_Server.ReceiveMessagesOnConnection(c.Key, netMessages, maxMessages);
                m_NetworkMessageConnectionSource = c.Key;
                readNetworkMessages();
            }
            HandleNewConnection(); //simple add to m_Connections if necessary
            if(GameManager.Instance.HasGameStarted)
            {
                ClientPlayer cPlayer = GameManager.Instance.ClientPlayer;
                if (cPlayer != null)
                {
                    PlayerUpdateMessage cPlayerUpdateMsg = new PlayerUpdateMessage(FindObjectOfType<ClientPlayer>().playerInfo, GameManager.Instance.m_PlayerUsername);
                    SendMessageToAllPlayers(cPlayerUpdateMsg);
                }
            }
        }
    }

    public void HandleNewConnection()
    {
        if (m_ConnectionToAdd.HasValue)
        {
            if (m_ConnectionToAdd.Value.Value == "") //kick them out
            {
                m_Server.CloseConnection(m_ConnectionToAdd.Value.Key);
            }
            else
            {
                m_Connections[m_ConnectionToAdd.Value.Key] = m_ConnectionToAdd.Value.Value;
                m_ConnectionToAdd = null;
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
    public float realignWidth = 8.0f;
    protected void RealignPlayersAndSend()
    {
        List<Player> players = GameManager.Instance.m_Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetPosition(new Vector2((-realignWidth * 0.25f) + ((realignWidth / players.Count) * i), players[i].transform.position.y));
            string playerID;
            if(players[i] is NetworkedPlayer)
            {
                playerID = (players[i] as NetworkedPlayer).playerID;
            }
            else
            {
                playerID = GameManager.Instance.m_PlayerUsername;
            }
            SendMessageToAllPlayers(new PlayerUpdateMessage(players[i].playerInfo, playerID), SendType.Reliable);
        }
    }

    protected override void HandleNetworkMessage(Message msg)
    {
        if(msg is PlayerConnectedMessage) //player sends this once connected to send name
        {
            PlayerConnectedMessage nMsg = msg as PlayerConnectedMessage;
            if(GameManager.Instance.GetPlayer(nMsg.playerID) != null) //player name already used, prepare to kick
            {
                m_ConnectionToAdd = new KeyValuePair<uint, string>(m_NetworkMessageConnectionSource, "");
            }
            else
            {
                m_ConnectionToAdd = new KeyValuePair<uint, string>(m_NetworkMessageConnectionSource, nMsg.playerID);
                GameManager.Instance.AddPlayer(nMsg.playerID);
                SendMessageToAllExceptPlayer(nMsg.playerID, msg, SendType.Reliable);
                Invoke("RealignPlayersAndSend", 0.1f); //need a delay for them to add the player
            }
        }
        if (msg is PlayerUpdateMessage)
        {
            PlayerUpdateMessage nMsg = msg as PlayerUpdateMessage;
            GameManager.Instance.UpdatePlayerInformation(ref nMsg.info, nMsg.playerID);
            SendMessageToAllExceptPlayer(nMsg.playerID, nMsg);
        }
        if(msg is ObstacleModifiedMessage)
        {
            Debug.Log("Obstacle Modified Message");
            ObstacleModifiedMessage nMsg = msg as ObstacleModifiedMessage;
            SendMessageToAllExceptPlayer(nMsg.playerID, nMsg, SendType.Reliable);
            (GameManager.Instance.m_AllObstacles[(int)nMsg.obstacleID] as IObstacle).InteractedWith(GameManager.Instance.GetPlayer(nMsg.playerID));
        }
        base.HandleNetworkMessage(msg);
    }

    
}
