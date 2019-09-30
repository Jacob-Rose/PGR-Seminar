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
}

public class GameStartMessage : Message
{
    public DateTime timeToStart;
    public GameStartMessage(byte[] buffer) // for client to decipher
    {
        
        //byte[] tickB = new byte[8];
        //Buffer.BlockCopy(buffer, 1, tickB, 0, 8);
        long ticks = BitConverter.ToInt64(buffer, 2);
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

    }

    public PlayerUpdateMessage() { }
    public override byte[] toBuffer()
    {
        throw new NotImplementedException();
    }
}