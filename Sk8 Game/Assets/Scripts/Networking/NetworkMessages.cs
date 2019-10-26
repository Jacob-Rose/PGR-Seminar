using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum NetworkEvent
{
    StartGame,
    InitSync, //to sync on first connect
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
        NetworkEvent eventType = (NetworkEvent)sho;
        switch (eventType)
        {
            case NetworkEvent.StartGame:
                msg = new GameStartMessage(msgBytes);
                break;
            case NetworkEvent.PlayerUpdateInfo:
                msg = new PlayerUpdateMessage(msgBytes);
                break;
            case NetworkEvent.PlayerConnected:
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
    public DateTime timeToStart; //universal time to start, usually a few seconds past DateTime.Now()
    public GameStartMessage(byte[] buffer) // for client to decipher
    {
        
        int currentByteIndex = 0;
        eventType = BitConverter.ToUInt16(buffer, 0);
        currentByteIndex += 2;
        long ticks = BitConverter.ToInt64(buffer, currentByteIndex); //first two used for eventType
        currentByteIndex += 8;
        timeToStart = new DateTime(ticks);
    }

    public GameStartMessage(long timeToStart) {
        this.timeToStart = new DateTime(timeToStart);
        eventType = (ushort)NetworkEvent.StartGame;
    }

    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[10];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(timeToStart.Ticks), 0, buffer, 2, 8);
        return buffer;
    }
}

public class PlayerUpdateMessage : Message
{
    public string playerID;
    public PlayerInfo info;
    public PlayerUpdateMessage(byte[] buffer)
    {
        info = new PlayerInfo();
        int currentByteIndex = 0;//event type took two bytes
        eventType = BitConverter.ToUInt16(buffer, currentByteIndex);
        currentByteIndex += sizeof(ushort); //2
        int playerIDCount = BitConverter.ToUInt16(buffer, currentByteIndex);
        playerID = "";
        for (int i = 0; i < playerIDCount; i++)
        {
            playerID += BitConverter.ToChar(buffer, currentByteIndex);
            currentByteIndex += sizeof(char);
        }
        currentByteIndex += playerID.Length * sizeof(char);
        info.zRot = BitConverter.ToSingle(buffer, currentByteIndex);
        currentByteIndex += sizeof(float); //10
        info.position.x = BitConverter.ToSingle(buffer, currentByteIndex);
        currentByteIndex += sizeof(float); //14
        info.position.y = BitConverter.ToSingle(buffer, currentByteIndex);
        currentByteIndex += sizeof(float); //18
    }

    public PlayerUpdateMessage(PlayerInfo info, string playerID) {
        eventType = (ushort)NetworkEvent.PlayerUpdateInfo;
        this.playerID = playerID;
        this.info = info;
    }
    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[14 + playerID.Length * sizeof(char)];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(playerID.Length), 0, buffer, 2, 4);
        for (int i = 0; i < playerID.Length; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(playerID[i]), 0, buffer, 4 + i * sizeof(char), sizeof(char));
        }
        Buffer.BlockCopy(BitConverter.GetBytes(info.zRot), 0, buffer, 6, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(info.position.x), 0, buffer, 10, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(info.position.y), 0, buffer, 14, 4);
        return buffer;
    }
}

public class PlayerConnectedMessage : Message
{
    public string playerID;
    public PlayerConnectedMessage(byte[] buffer)
    {
        int currentByteIndex = 0;
        eventType = BitConverter.ToUInt16(buffer, currentByteIndex);
        currentByteIndex += sizeof(ushort);
        int playerIDCount = BitConverter.ToUInt16(buffer, currentByteIndex);
        playerID = "";
        for(int i = 0; i < playerIDCount; i++)
        {
            playerID += BitConverter.ToChar(buffer, currentByteIndex);
            currentByteIndex += sizeof(char);
        }
        currentByteIndex += playerID.Length * sizeof(char);
    }

    public PlayerConnectedMessage(string playerID) {
        eventType = (ushort)NetworkEvent.PlayerConnected;
        this.playerID = playerID;
    }
    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[4 + (playerID.Length * sizeof(char))];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(playerID.Length), 0, buffer, 2, 4);
        for(int i = 0; i < playerID.Length; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(playerID[i]), 0, buffer, 4 + i * sizeof(char), sizeof(char));
        }
        return buffer;
    }
}

public class PlayerDisconnectedMessage : Message
{
    public string playerID;
    public PlayerDisconnectedMessage(byte[] buffer)
    {
        int currentByteIndex = 0;
        eventType = BitConverter.ToUInt16(buffer, currentByteIndex);
        currentByteIndex += sizeof(ushort);
        int playerIDCount = BitConverter.ToUInt16(buffer, currentByteIndex);
        playerID = "";
        for (int i = 0; i < playerIDCount; i++)
        {
            playerID += BitConverter.ToChar(buffer, currentByteIndex);
            currentByteIndex += sizeof(char);
        }
        currentByteIndex += playerID.Length * sizeof(char);
    }

    public PlayerDisconnectedMessage(string playerID)
    {
        eventType = (ushort)NetworkEvent.PlayerDisconnected;
        this.playerID = playerID;
    }
    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[4 + (playerID.Length * sizeof(char))];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(playerID.Length), 0, buffer, 2, 4);
        for (int i = 0; i < playerID.Length; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(playerID[i]), 0, buffer, 4 + i * sizeof(char), sizeof(char));
        }
        return buffer;
    }
}

/*
public class InitSyncMessage : Message
{
    public List<PlayerInfo> playerIDs;
    InitSyncMessage(List<PlayerInfo> playerIDs)
    {
        this.playerIDs = playerIDs;
    }

    InitSyncMessage(byte[] buffer)
    {
        int currentByteIndex = 0;
        eventType = BitConverter.ToUInt16(buffer, (int)currentByteIndex);
        currentByteIndex += 2;
        ushort count = BitConverter.ToUInt16(buffer, (int)currentByteIndex);
        currentByteIndex += 2;
        for (int i = 0; i < count; i++)
        {
            PlayerInfo info = new PlayerInfo();
            info.id = BitConverter.ToInt32(buffer, currentByteIndex);
            currentByteIndex += 4;
            info.zRot = BitConverter.ToSingle(buffer, currentByteIndex);
            currentByteIndex += 4;
            info.position.x = BitConverter.ToSingle(buffer, currentByteIndex);
            currentByteIndex += 4;
            info.position.y = BitConverter.ToSingle(buffer, currentByteIndex);
            currentByteIndex += 4;
            playerIDs.Add();
        }
    }

    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[(playerIDs.Count * 4) + 4];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes((ushort)playerIDs.Count), 0, buffer, 2, 2);
        for (int i = 0; i < playerIDs.Count; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(playerIDs[i].id), 0, buffer, 4 + (i * 16), 4);
            Buffer.BlockCopy(BitConverter.GetBytes(playerIDs[i].zRot), 0, buffer, 8 + (i * 16), 4);
            Buffer.BlockCopy(BitConverter.GetBytes(playerIDs[i].position.x), 0, buffer, 12 + (i * 16), 4);
            Buffer.BlockCopy(BitConverter.GetBytes(playerIDs[i].position.y), 0, buffer, 16 + (i * 16), 4);
        }
        return buffer;
    }
}
*/