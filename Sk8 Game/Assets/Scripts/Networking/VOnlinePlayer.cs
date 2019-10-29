using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Valve.Sockets;
//yang of VHostBehavior


public class VOnlinePlayer : Networked
{
    protected uint m_Connection = uint.MaxValue;

    public static VOnlinePlayer m_Instance;
    public override void Start()
    {
        m_Instance = this;
        m_Status = (info, context) =>
        {
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    print("no connection exist, reset?");
                    break;

                case ConnectionState.Connected:
                    Debug.Log("I, the Client, connected to server - ID: " + info.connection);
                    m_Connection = info.connection;
                    m_Server.SendMessageToConnection(m_Connection, new PlayerConnectedMessage(GameManager.Instance.m_PlayerUsername).toBuffer(), SendType.Reliable);
                    break;

                case ConnectionState.ClosedByPeer:
                    m_Server.CloseConnection(m_Connection);
                    m_Connection = uint.MaxValue;
                    Debug.Log("I, the Client, disconnected from server");
                    break;

                case ConnectionState.ProblemDetectedLocally:
                    m_Server.CloseConnection(m_Connection);
                    m_Connection = uint.MaxValue;
                    Debug.Log("I, the Client, unable to connect");
                    break;
            }
        };
        base.Start();
    }

    public override void OnDestroy()
    {
        m_Server.CloseConnection(m_Connection);
        base.OnDestroy();
    }

    public override void Update()
    {
        if (m_Server != null && m_Status != null)
        {
            m_Server.DispatchCallback(m_Status);
            netMessageCount = m_Server.ReceiveMessagesOnConnection(m_Connection, netMessages, maxMessages);
        }
        readNetworkMessages();
        ClientPlayer player = FindObjectOfType<ClientPlayer>();
        m_Server.SendMessageToConnection(m_Connection, new PlayerUpdateMessage(player.playerInfo, GameManager.Instance.m_PlayerUsername).toBuffer(), SendType.NoDelay);
    }

    protected override void HandleNetworkMessage(Message msg)
    {
        if(msg is GameStartMessage)
        {
            GameStartMessage nMsg = msg as GameStartMessage;
            GameManager.Instance.StartGameInSeconds((float)(nMsg.timeToStart - DateTime.Now).TotalSeconds);
        }
        if(msg is PlayerConnectedMessage)
        {
            PlayerConnectedMessage nMsg = msg as PlayerConnectedMessage;
            NetworkedPlayer p = GameManager.Instance.AddPlayer(nMsg.playerID);
            p.playerID = nMsg.playerID;
            
        }
        else if(msg is PlayerDisconnectedMessage)
        {
            PlayerDisconnectedMessage nMsg = msg as PlayerDisconnectedMessage;
            GameManager.Instance.RemovePlayer(nMsg.playerID);
        }
        else if(msg is PlayerUpdateMessage)
        {
            PlayerUpdateMessage nMsg = msg as PlayerUpdateMessage;
            GameManager.Instance.UpdatePlayerInformation(ref nMsg.info, nMsg.playerID);
        }
    }

    public void ConnectToIP(string ip)
    {
        IPAddress address;
        if(IPAddress.TryParse(ip, out address))
        {
            m_Address.SetAddress(ip, m_Port);
            m_Connection = m_Server.Connect(ref m_Address);
        }
        else
        {
            Debug.LogError("Ip not valid");
        }
        
    }
}
