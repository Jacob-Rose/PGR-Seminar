﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

public class ServerBehavior : MonoBehaviour
{
    /* NOTE: I GOT NO CLUE HOW NETWORKING WORKS, SO MOST CODE IS BASED OF
     * https://github.com/Unity-Technologies/multiplayer/blob/master/com.unity.transport/Documentation~/workflow-client-server.md
     * god bless this dude too for having up to date docs
     * https://gist.github.com/duynguye/7b6b0828a89ab4e4b7da769f86adc1fe
     */
    public UdpNetworkDriver m_Driver;
    public NativeList<NetworkConnection> m_Connections;
    public List<NetworkedPlayer> m_Players;
    public NetworkPipeline m_Pipeline;
    public NetworkEndPoint m_Endpoint;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        m_Driver = new UdpNetworkDriver(new SimulatorUtility.Parameters { MaxPacketSize = 256, MaxPacketCount = 30, PacketDelayMs = 100 });
        m_Pipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage), typeof(SimulatorPipelineStage));

        m_Endpoint = new NetworkEndPoint();
        m_Endpoint = NetworkEndPoint.Parse("0.0.0.0", 9000);

        if (m_Driver.Bind(m_Endpoint) != 0)
        {
            Debug.Log("Failed to bind to port 9000");
        }
        else
        {
            m_Driver.Listen();
        }
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
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
            HandlePlayerConnected(c);
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

                    using (var writer = new DataStreamWriter(4, Allocator.Temp))
                    {
                        writer.Write(number);
                        m_Driver.Send(m_Pipeline, m_Connections[i], writer);
                    }
                }
            }
        }
    }

    public void HandleDataMessage(ref DataStreamReader stream)
    {

    }
    public void HandlePlayerConnected(NetworkConnection c)
    {
        m_Connections.Add(c);
        Debug.Log("Accepted a connection");
    }
}
