using System.Collections;
using System.Collections.Generic;
using Valve.Sockets;
using UnityEngine;
using AOT;
using System.Text;
using System;




public class VClientBehavior : MonoBehaviour
{
    NetworkingSockets m_Client = new NetworkingSockets();
    StatusCallback m_Status;
    Address m_Address = new Address();
    public const ushort port = 9000;
    uint m_Connection = uint.MaxValue;

    private const int maxMessages = 20;
    private NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

    byte[] messageDataBuffer = new byte[256];

    static VClientBehavior m_Instance;

    void Awake()
    {
        InitializeValveSockets();
        m_Instance = this;
    }

    void OnApplicationQuit()
    {
        DeinitializeValveSockets();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Status = OnClientStatusUpdate;
    }

    [MonoPInvokeCallback(typeof(StatusCallback))]
    static void OnClientStatusUpdate(StatusInfo info, System.IntPtr context)
    {
        switch (info.connectionInfo.state)
        {
            case ConnectionState.None:
                break;

            case ConnectionState.Connected:
                Debug.Log("Client connected to server - ID: " + m_Instance.m_Connection);
                break;

            case ConnectionState.ClosedByPeer:
                m_Instance.m_Client.CloseConnection(m_Instance.m_Connection);
                Debug.Log("Client disconnected from server");
                break;

            case ConnectionState.ProblemDetectedLocally:
                m_Instance.m_Client.CloseConnection(m_Instance.m_Connection);
                Debug.Log("Client unable to connect");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Connection == uint.MaxValue)
        {
            m_Client.DispatchCallback(m_Status);

            int netMessagesCount = m_Client.ReceiveMessagesOnConnection(m_Connection, netMessages, maxMessages);
            if (netMessagesCount > 0)
            {
                for (int i = 0; i < netMessagesCount; i++)
                {
                    ref NetworkingMessage netMessage = ref netMessages[i];

                    Debug.Log("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                    netMessage.CopyTo(messageDataBuffer);
                    netMessage.Destroy();

                    Message m = ConvertMessageBuffer();
                }
            }
        }
    }

    public Message ConvertMessageBuffer()
    {
        if(BitConverter.IsLittleEndian)
        {
            Array.Reverse(messageDataBuffer);
        }
        byte[] eventTypeBytes = new byte[2];
        Buffer.BlockCopy(messageDataBuffer, 0, eventTypeBytes, 0, 2);
        
        ushort sho = BitConverter.ToUInt16(eventTypeBytes, 0);
        EventTypes eventType = (EventTypes)sho;
        Message msg;
        switch(eventType)
        {
            case EventTypes.StartGame:
                msg = new GameStartMessage(messageDataBuffer);
                break;
            case EventTypes.PlayerUpdateInfo:
                msg = new 
                break;
        }
        return msg;
    }

    
    

    public void ConnectToIP(string ip)
    {
        m_Address.SetAddress(ip, port);
        m_Connection = m_Client.Connect(ref m_Address);
    }

    public static void InitializeValveSockets()
    {
        Valve.Sockets.Library.Initialize();
    }

    public static void DeinitializeValveSockets()
    {
        Valve.Sockets.Library.Deinitialize();
    }
}
