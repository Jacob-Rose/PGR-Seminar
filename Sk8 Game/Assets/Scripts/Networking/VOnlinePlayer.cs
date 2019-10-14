using AOT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Sockets;
//yang of VHostBehavior
public class VOnlinePlayer : Networked
{
    private StatusCallback m_Status;
    protected ConnectionInfo m_Connection = new ConnectionInfo(uint.MaxValue);

    public static VOnlinePlayer m_Instance;

    [MonoPInvokeCallback(typeof(StatusCallback))]
    static void OnClientStatusUpdate(StatusInfo info, System.IntPtr context)
    {
        switch (info.connectionInfo.state)
        {
            case ConnectionState.None:
                break;

            case ConnectionState.Connected:
                Debug.Log("I, the Client, connected to server - ID: " + info.connection);
                m_Instance.m_Connection = new ConnectionInfo(info.connection); //the server equals one person
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
    }
    protected override void HandleNetworkMessage(Message msg)
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
    public void ConnectToIP(string ip)
    {
        m_Address.SetAddress(ip, m_Port);
        m_Connection = new ConnectionInfo(m_Server.Connect(ref m_Address));
    }

    
}
