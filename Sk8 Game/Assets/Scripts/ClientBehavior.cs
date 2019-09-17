﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;


public class ClientBehavior : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public NetworkPipeline m_Pipeline;
    public NetworkEndPoint m_Endpoint;
    public PlayerManager m_Players;
    public bool Done;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void Start()
    {
        m_Driver = new UdpNetworkDriver(new SimulatorUtility.Parameters { MaxPacketSize = 256, MaxPacketCount = 30, PacketDelayMs = 100 });
        m_Pipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage), typeof(SimulatorPipelineStage));
        m_Connection = default;
    }

    public void ConnectToIP(string ip)
    {
        m_Endpoint = new NetworkEndPoint();
        m_Endpoint = NetworkEndPoint.Parse(ip, 9000);

        m_Connection = m_Driver.Connect(m_Endpoint);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    // Update is called once per frame
    public void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!Done)
            {
                Debug.Log("Something went wrong during connection");
            }

            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");

                var value = 1;

                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    m_Connection.Send(m_Driver, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                uint value = stream.ReadUInt(ref readerCtx);

                Debug.Log("Got the value = " + value + " back from the server.");

                Done = true;
                m_Connection.Disconnect(m_Driver);
                m_Connection = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from the server.");
                m_Connection = default;
            }
        }
    }
}
