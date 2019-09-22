using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Valve.Sockets;
using UnityEngine;

public enum ClientNetworkMessages
{
    SetupInfo, //the first signal after connection to sync data
    StartGame,
    PlayerInfo,
    PlayerJoined,
    PlayerLeft,
    TileCreated, //tile destruction and recycling done without server intervention
}


public class ClientBehavior : MonoBehaviour
{
    //public NetworkingSockets m_ClientSocket;
    //public Address m_Address;
    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public NetworkPipeline m_Pipeline;
    public NetworkEndPoint m_Endpoint;
    public PlayerManager m_PlayerManager;
    public GameManager m_Gamemanager;

    public void Awake()
    {
        //Valve.Sockets.Library.Initialize(); //both client and host player both have clientbehavior
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        //Valve.Sockets.Library.Deinitialize();
    }
    public void Start()
    {
        m_Driver = new UdpNetworkDriver(new SimulatorUtility.Parameters { MaxPacketSize = 256, MaxPacketCount = 30, PacketDelayMs = 100 });
        m_Pipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage), typeof(SimulatorPipelineStage));
        m_Connection = default;
        m_PlayerManager.AddClientPlayer(); //add self
    }

    public void ConnectToIP(string ip)
    {
        m_Endpoint = new NetworkEndPoint();
        m_Endpoint = NetworkEndPoint.Parse(ip, 9000);

        m_Connection = m_Driver.Connect(m_Endpoint);
    }

    

    // Update is called once per frame
    public void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");

                //todo add username info sent
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                int dataType = stream.ReadInt(ref readerCtx);
                if(dataType == (int)ClientNetworkMessages.SetupInfo)
                {
                    int playerCount = stream.ReadInt(ref readerCtx);
                    for(int i =0; i < playerCount; i++)
                    {
                        PlayerInfo info = ServerBehavior.ReadPlayerInfo(ref stream);
                        m_PlayerManager.AddNetworkedPlayer(info);
                    }
                }
                else if(dataType == (int)ClientNetworkMessages.PlayerInfo)
                {
                    PlayerInfo newInfo = ServerBehavior.ReadPlayerInfo(ref stream);
                    m_PlayerManager.HandlePlayerUpdate(newInfo);
                }
                else if(dataType == (int)ClientNetworkMessages.StartGame)
                {
                    byte[] time = stream.ReadBytesAsArray(ref readerCtx, 4);
                    long tickTime = BitConverter.ToInt64(time, 0);
                    DateTime timeToStart = new DateTime(tickTime);
                    Invoke("StartGame", (timeToStart - DateTime.Now).Seconds);
                }
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

    public void StartGame()
    {
        //todo
    }
}
