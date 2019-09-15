using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerBehavior : MonoBehaviour
{
    /* NOTE: I GOT NO CLUE HOW NETWORKING WORKS, SO MOST CODE IS BASED OF
     * https://github.com/Unity-Technologies/multiplayer/blob/master/com.unity.transport/Documentation~/workflow-client-server.md
     */
    public UdpNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;
    private NetworkPipeline m_NetworkPipeline;
    // Start is called before the first frame update
    void Start()
    {
        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        m_NetworkPipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage), typeof(SimulatorPipelineStage));
        if (m_Driver.Bind(new NetworkEndPoint()) != 0)
        {
            Debug.Log("Failed to bind to port 9000");
        }
        else
        {
            m_Driver.Listen();
        }
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        //ensure that all events are handeled in network driver before anything is done
        m_Driver.ScheduleUpdate().Complete();
        //remove any stale connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }
        //accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
        }
        //now driver is up to date, do fun stuff now
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
                continue;
            NetworkEvent.Type cmd;
            //go through all received network events until its empty
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) !=
                NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    uint number = stream.ReadUInt(ref readerCtx);
                    Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;

                    using (var writer = new DataStreamWriter(4, Allocator.Temp))
                    {
                        writer.Write(number);
                        m_Driver.Send(m_NetworkPipeline, m_Connections[i], writer);
                    }
                }
            }
        }
    }
}
