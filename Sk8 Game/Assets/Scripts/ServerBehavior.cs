using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Valve.Sockets;
using UnityEngine;


public enum ServerNetworkMessages
{
    PlayerUpdate,
    PlayerDisconnect,
    PlayerConnect
}
public struct ServerPlayer
{
    public ServerPlayer(NetworkConnection connection)
    {
        this.connection = connection;
        info.currentScore = 0;
        info.currentSpeed = 0.0f;
        info.id = connection.InternalId; //only used to id, this ensures no duplicates cheaply but can be random nums for id, or even strings for usernames
        info.move = PlayerMove.NONE;
        info.position = Vector3.zero;
        info.zRot = 0.0f;
    }

    public ServerPlayer(NetworkConnection connection, PlayerInfo info)
    {
        this.info = info;
        this.connection = connection;
    }
    public PlayerInfo info;
    public NetworkConnection connection;
}

public class ServerBehavior : MonoBehaviour
{
    /* NOTE: I GOT NO CLUE HOW NETWORKING WORKS, SO MOST CODE IS BASED OF
     * https://github.com/Unity-Technologies/multiplayer/blob/master/com.unity.transport/Documentation~/workflow-client-server.md
     * god bless this dude too for having up to date docs
     * https://gist.github.com/duynguye/7b6b0828a89ab4e4b7da769f86adc1fe
     */
    public UdpNetworkDriver m_Driver;
    public NativeList<ServerPlayer> m_Players;
    public NetworkPipeline m_Pipeline;
    public NetworkEndPoint m_Endpoint;
    public bool gameStarted = false;


    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        m_Driver = new UdpNetworkDriver(new SimulatorUtility.Parameters { MaxPacketSize = 256, MaxPacketCount = 30, PacketDelayMs = 50 });
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
        m_Players = new NativeList<ServerPlayer>(16, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Players.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        //ensure that all events are handeled in network driver before anything is done
        m_Driver.ScheduleUpdate().Complete();

        //remove any stale connections
        RemoveStaleConnections();
        //accept new connections
        CheckNewIncomingConnections();
        //now driver is up to date, do fun stuff now
        
        for (int i = 0; i < m_Players.Length; i++)
        {
            DataStreamReader stream;
            if (!m_Players[i].connection.IsCreated)
                continue;
            NetworkEvent.Type cmd;
            //go through all received network events until its empty
            while ((cmd = m_Driver.PopEventForConnection(m_Players[i].connection, out stream)) !=
                NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    int dataType = stream.ReadInt(ref readerCtx);
                    if(dataType == (int)ServerNetworkMessages.PlayerUpdate)
                    {
                        PlayerInfo newInfo = ReadPlayerInfo(ref stream);
                        m_Players[i] = new ServerPlayer(m_Players[i].connection, newInfo);
                        DataStreamWriter writer = new DataStreamWriter();
                        PlayerInfoToNetStream(newInfo, ref writer);
                        for (int j = 0; j < m_Players.Length; j++)
                        {
                            if (j == i)
                                continue;

                            m_Driver.Send(m_Pipeline, m_Players[j].connection, writer);
                        }
                    }
                }
            }
        }
    }

    public static PlayerInfo ReadPlayerInfo(ref DataStreamReader stream)
    {
        PlayerInfo player;
        var readerCtx = default(DataStreamReader.Context);
        player.id = stream.ReadInt(ref readerCtx);
        player.currentScore = stream.ReadInt(ref readerCtx);
        player.currentSpeed = stream.ReadFloat(ref readerCtx);
        player.move = (PlayerMove)stream.ReadInt(ref readerCtx);
        player.position.x = stream.ReadFloat(ref readerCtx);
        player.position.y = stream.ReadFloat(ref readerCtx);
        player.zRot = stream.ReadFloat(ref readerCtx);
        return player;
    }
    public static void PlayerInfoToNetStream(PlayerInfo info, ref DataStreamWriter writer)
    {
        writer.Write(info.id);
        writer.Write(info.currentScore);
        writer.Write(info.currentSpeed);
        writer.Write((int)info.move);
        writer.Write(info.position.x);
        writer.Write(info.position.y);
        writer.Write(info.zRot);
    }

    public void RemoveStaleConnections()
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (!m_Players[i].connection.IsCreated)
            {
                m_Players.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    public void CheckNewIncomingConnections()
    {
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            HandlePlayerConnected(c);
        }
    }

    public void HandlePlayerConnected(NetworkConnection c)
    {
        ServerPlayer player = new ServerPlayer(c);
        m_Players.Add(player);
        
        Debug.Log("Accepted a connection");
    }

    public void SendStartupInfo(ServerPlayer player)
    {
        DataStreamWriter writer = new DataStreamWriter();
        getStartupInfoStream(ref writer);
        m_Driver.Send(m_Pipeline, player.connection, writer);
    }

    public void getStartupInfoStream(ref DataStreamWriter writer)
    {
        writer.Write(m_Players.Length);
        for(int i = 0; i < m_Players.Length; i++)
        {
            PlayerInfoToNetStream(m_Players[i].info, ref writer);
        }
    }

    public void setGameStartSignal()
    {
        if(!gameStarted)
        {
            DateTime timeToStart = DateTime.Now; //global time
            timeToStart.AddSeconds(3.0f);//add three seconds to start
            DataStreamWriter writer = new DataStreamWriter();
            writer.Write((int)ClientNetworkMessages.StartGame);
            writer.Write(timeToStart.Ticks);
        }
    }
}
