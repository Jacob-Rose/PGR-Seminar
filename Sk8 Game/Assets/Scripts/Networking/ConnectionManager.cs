using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager 
{
    private List<ConnectionInfo> m_Connections = new List<ConnectionInfo>();

    public NetworkedPlayer NetworkedPlayerPrefab;

    public ConnectionInfo addConnection(uint connection)
    {
        ConnectionInfo info = new ConnectionInfo(connection);
        m_Connections.Add(info);
        return info;
    }

    public void removeConnection(uint connection)
    {
        for(int i = 0; i < m_Connections.Count; i++)
        {
            if(m_Connections[i].connection == connection)
            {
                m_Connections.RemoveAt(i);
                break;
            }
        }
    }

    public ConnectionInfo getConnection(int index)
    {
        return m_Connections[index];
    }

    public int getConnectionCount()
    {
        return m_Connections.Count;
    }

    public uint getConnectionFromPlayerID(int playerID)
    {
        for(int i = 0; i < m_Connections.Count; i++)
        {
            if(m_Connections[i].player.playerID == playerID)
            {
                return m_Connections[i].connection;
            }
        }
        throw new System.Exception("Connection not found from playerID");
    }
}

public class ConnectionInfo
{
    public ConnectionInfo(uint connection)
    {
        this.connection = connection;
        this.player = Object.Instantiate(((GameObject)Resources.Load("Prefabs/NetworkPlayer"))).GetComponent<NetworkedPlayer>();
    }
    public uint connection;
    public NetworkedPlayer player;
}
