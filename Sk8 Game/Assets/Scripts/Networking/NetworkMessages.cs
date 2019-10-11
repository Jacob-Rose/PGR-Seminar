using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EventTypes
{
    StartGame,
    PlayerConnected,
    PlayerDisconnected,
    PlayerUpdateInfo,
    ObstacleGenerated,
    ObstacleModified
}

public abstract class Message
{
    //first two bytes is for event type
    public ushort eventType;
    public abstract byte[] toBuffer();

    public static Message decipherMessage(byte[] msgBytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            //Array.Reverse(msgBytes);
        }
        Message msg;
        byte[] eventTypeBytes = new byte[2];
        Buffer.BlockCopy(msgBytes, 0, eventTypeBytes, 0, 2);

        ushort sho = BitConverter.ToUInt16(eventTypeBytes, 0);
        EventTypes eventType = (EventTypes)sho;
        switch (eventType)
        {
            case EventTypes.StartGame:
                msg = new GameStartMessage(msgBytes);
                break;
            case EventTypes.PlayerUpdateInfo:
                msg = new PlayerUpdateMessage(msgBytes);
                break;
            case EventTypes.PlayerConnected:
                msg = new PlayerConnectedMessage(msgBytes);
                break;
            default:
                throw new Exception("oops");
        }
        return msg;
    }
}

public class GameStartMessage : Message
{
    public DateTime timeToStart;
    public GameStartMessage(byte[] buffer) // for client to decipher
    {
        
        //byte[] tickB = new byte[8];
        //Buffer.BlockCopy(buffer, 1, tickB, 0, 8);
        long ticks = BitConverter.ToInt64(buffer, 2); //first two used for eventType
        timeToStart = new DateTime(ticks);

    }

    public GameStartMessage() { } //for server to send

    public override byte[] toBuffer()
    {
        byte[] bytes = new byte[10];
        Buffer.BlockCopy(BitConverter.GetBytes((ushort)EventTypes.StartGame), 0, bytes, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(timeToStart.Ticks), 0, bytes, 2, 8);
        return bytes;
    }
}

public class PlayerUpdateMessage : Message
{
    public PlayerUpdateMessage(byte[] bytes)
    {
        PlayerInfo info = new PlayerInfo();
        byte[] score = new byte[4];

    }

    public PlayerUpdateMessage() { }
    public override byte[] toBuffer()
    {
        throw new NotImplementedException();
    }
}

public class PlayerConnectedMessage : Message
{
    public uint playerID;
    public PlayerConnectedMessage(byte[] bytes)
    {
        int byteCount = 2;//event type took two bytes
        playerID = BitConverter.ToUInt32(bytes, byteCount);
        byteCount += 4;
    }

    public PlayerConnectedMessage() { }
    public override byte[] toBuffer()
    {
        throw new NotImplementedException();
    }
}