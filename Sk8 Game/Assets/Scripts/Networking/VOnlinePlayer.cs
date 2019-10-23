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
    protected ConnectionInfo m_Connection = new ConnectionInfo(uint.MaxValue, null);

    public static VOnlinePlayer m_Instance;
    public override void Start()
    {
        m_Instance = this;
        m_Status = (info, context) =>
        {
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    print("no conn");
                    break;

                case ConnectionState.Connected:
                    Debug.Log("I, the Client, connected to server - ID: " + info.connection);
                    m_Instance.m_Connection = new ConnectionInfo(info.connection, Instantiate(((GameObject)Resources.Load("Prefabs/NetworkedPlayer"))).GetComponent<NetworkedPlayer>()); //the server equals one person
                    break;

                case ConnectionState.ClosedByPeer:
                    m_Instance.m_Server.CloseConnection(m_Instance.m_Connection.connection);
                    Debug.Log("I, the Client, disconnected from server");
                    break;

                case ConnectionState.ProblemDetectedLocally:
                    m_Instance.m_Server.CloseConnection(m_Instance.m_Connection.connection);
                    Debug.Log("I, the Client, unable to connect");
                    break;
            }
        };
        base.Start();
    }
    protected override void HandleNetworkMessage(Message msg)
    {
        if(msg is GameStartMessage)
        {
            GameStartMessage nMsg = msg as GameStartMessage;
            Invoke("StartGame", (float)(nMsg.timeToStart - DateTime.Now).TotalSeconds);
        }
        else if(msg is PlayerConnectedMessage)
        {
            PlayerConnectedMessage nMsg = msg as PlayerConnectedMessage;
            GameObject newPlayer = Resources.Load("Prefabs/NetworkedPlayer") as GameObject;
            Toolbox.Instance.addPlayer(newPlayer.GetComponent<NetworkedPlayer>());
        }
        else if(msg is PlayerDisconnectedMessage)
        {
            PlayerDisconnectedMessage nMsg = msg as PlayerDisconnectedMessage;
            Player player = Toolbox.Instance.removePlayer(nMsg.playerID);
            Destroy(player.gameObject);
        }
    }

    public void ConnectToIP(string ip)
    {
        IPAddress address;
        if(IPAddress.TryParse(ip, out address ))
        {
            m_Address.SetAddress(ip, m_Port);
            m_Connection = createNetworkPlayer(m_Server.Connect(ref m_Address));
        }
        else
        {
            Debug.LogError("Ip not valid");
        }
        
    }

    public ConnectionInfo createNetworkPlayer(uint connection)
    {
        return new ConnectionInfo(connection, Instantiate(((GameObject)Resources.Load("Prefabs/NetworkedPlayer"))).GetComponent<NetworkedPlayer>());
    }


}
