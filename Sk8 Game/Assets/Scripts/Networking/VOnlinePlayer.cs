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
    private StatusCallback m_Status;
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
                    break;

                case ConnectionState.Connected:
                    Debug.Log("I, the Client, connected to server - ID: " + info.connection);
                    m_Instance.m_Connection = new ConnectionInfo(info.connection, Instantiate(((GameObject)Resources.Load("Prefabs/NetworkPlayer"))).GetComponent<NetworkedPlayer>()); //the server equals one person
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
            m_Server.Connect(ref m_Address);
        }
        else
        {
            Debug.LogError("Ip not valid");
        }
        
    }

    public bool TryParseIP(string ip, out IPAddress iPAddress)
    {
        try
        {
            ulong ipLong = 0;
            int currentIndex = 0;
            int[] ipVals = new int[4];
            for (int i = 0; i < 4; i++)
            {
                string numIp = ip.Substring(currentIndex, ip.IndexOf('.') - currentIndex);
                currentIndex = ip.IndexOf('.') + 1;
                int num;
                if (int.TryParse(numIp, out num))
                {
                    ipVals[i] = num;
                }
                else
                {
                    iPAddress = null;
                    return false;
                }

            }
            //ipLong = ipToInt(ipVals[0], ipVals[1], ipVals[2], ipVals[3]);
            iPAddress = new IPAddress(0);
            return true;
        }
        catch(Exception e)
        {
            iPAddress = null;
            return false;
        }
        
    }

    int ipToInt(int first, int second,
    int third, int fourth)
    {
        return (first << 24) | (second << 16) | (third << 8) | (fourth);
    }


}
