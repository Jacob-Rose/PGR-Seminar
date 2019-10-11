using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager 
{
    private List<ConnectionInfo> m_Connections = new List<ConnectionInfo>();

    public void addConnection(uint connection)
    {
        m_Connections.Add(new ConnectionInfo(connection));
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

    public int getConnectionCount()
    {
        return m_Connections.Count;
    }
}

public class ConnectionInfo
{
    public ConnectionInfo(uint connection)
    {
        this.connection = connection;
    }
    public uint connection;
    public ServerPlayer player;
}
