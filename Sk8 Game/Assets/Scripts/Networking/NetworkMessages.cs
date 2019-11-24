using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

enum NetworkEvent
{
    StartGame,
    PlayerConnected,
    PlayerDisconnected,
    PlayerUpdateInfo,
    PlayerFellBehind,
    PlayerHitObstacle,
    PlayerWonRace,
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
            case NetworkEvent.ObstacleGenerated:
                msg = new ObstacleGeneratedMessage(msgBytes);
                break;
            case NetworkEvent.PlayerFellBehind:
                msg = new PlayerFellBehindMessage(msgBytes);
                break;
            case NetworkEvent.ObstacleModified:
                msg = new ObstacleModifiedMessage(msgBytes);
                break;
            case NetworkEvent.PlayerWonRace:
                msg = new PlayerWonMessage(msgBytes);
                break;
            default:
                throw new Exception("oops");
        }
        return msg;
    }

}

public class GameStartMessage : Message
{
    public DateTime uniformTime; //universal time to start, to sync
    public float timeAfterToSpawn; //how much time after uniformTime do we start
    public GameStartMessage(byte[] buffer) // for client to decipher
    {
        int currentByteIndex = 0;
        eventType = BitConverter.ToUInt16(buffer, 0);
        currentByteIndex += sizeof(ushort);
        uniformTime = new DateTime(BitConverter.ToInt64(buffer, currentByteIndex)); //ticks is sent
        currentByteIndex += sizeof(long);
        timeAfterToSpawn = BitConverter.ToSingle(buffer, currentByteIndex);
        currentByteIndex += sizeof(float);
    }

    public GameStartMessage(long ticks, float timeAfterToSpawn) {
        uniformTime = new DateTime(ticks);
        eventType = (ushort)NetworkEvent.StartGame;
        this.timeAfterToSpawn = timeAfterToSpawn;
    }

    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[14];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(uniformTime.Ticks), 0, buffer, 2, 8);
        Buffer.BlockCopy(BitConverter.GetBytes(timeAfterToSpawn), 0, buffer, 10, 4);
        return buffer;
    }
}

public class PlayerUpdateMessage : Message
{
    public string playerID;
    public PlayerInfo info;
    public PlayerUpdateMessage(byte[] buffer)
    {
        //Could be optimized without copying buffers, but that would make it so much harder to fix bugs for
        byte[] eventTypeBuffer = new byte[sizeof(ushort)];
        byte[] playerIDLengthBuffer = new byte[sizeof(ushort)];
        int currentIndex = 0;
        Buffer.BlockCopy(buffer, currentIndex, eventTypeBuffer, 0, eventTypeBuffer.Length);
        currentIndex += eventTypeBuffer.Length;
        Buffer.BlockCopy(buffer, currentIndex, playerIDLengthBuffer, 0, playerIDLengthBuffer.Length);
        currentIndex += playerIDLengthBuffer.Length;
        int playerIDBufferCount = BitConverter.ToUInt16(playerIDLengthBuffer, 0);
        byte[] playerIDBuffer = new byte[playerIDBufferCount]; //we know length of string now (in bytes)
        Buffer.BlockCopy(buffer, currentIndex, playerIDBuffer, 0, playerIDBuffer.Length);
        currentIndex += playerIDBuffer.Length;
        byte[] zRotBuffer = new byte[sizeof(float)];
        Buffer.BlockCopy(buffer, currentIndex, zRotBuffer, 0, zRotBuffer.Length);
        currentIndex += zRotBuffer.Length;
        byte[] xPosBuffer = new byte[sizeof(float)];
        Buffer.BlockCopy(buffer, currentIndex, xPosBuffer, 0, xPosBuffer.Length);
        currentIndex += xPosBuffer.Length;
        byte[] yPosBuffer = new byte[sizeof(float)];
        Buffer.BlockCopy(buffer, currentIndex, yPosBuffer, 0, yPosBuffer.Length);
        currentIndex += yPosBuffer.Length;
        byte[] speedBuffer = new byte[sizeof(float)];
        Buffer.BlockCopy(buffer, currentIndex, speedBuffer, 0, speedBuffer.Length);
        currentIndex += speedBuffer.Length;
        byte[] colliderBuffer = new byte[sizeof(bool)];
        Buffer.BlockCopy(buffer, currentIndex, colliderBuffer, 0, colliderBuffer.Length);
        currentIndex += colliderBuffer.Length;
        byte[] scoreBuffer = new byte[sizeof(int)];
        Buffer.BlockCopy(buffer, currentIndex, scoreBuffer, 0, scoreBuffer.Length);
        currentIndex += scoreBuffer.Length;

        playerID = Encoding.ASCII.GetString(playerIDBuffer);
        Player p = GameManager.Instance.GetPlayer(playerID);
        if(p != null)
        {
            info = p.playerInfo;
            info.position.x = BitConverter.ToSingle(xPosBuffer, 0);
            info.position.y = BitConverter.ToSingle(yPosBuffer, 0);
            info.zRot = BitConverter.ToSingle(zRotBuffer, 0);
            info.currentSpeed = BitConverter.ToSingle(speedBuffer, 0);
            info.collidable = BitConverter.ToBoolean(colliderBuffer, 0);
            info.currentScore = BitConverter.ToInt32(scoreBuffer, 0);
        }
        else
        {
            if(GameManager.Instance.HasPlayerExisted(playerID))
            {
                Debug.LogWarning("Player has been deleted");
            }
            else
            {
                Debug.LogError("Who Dat Boy? Who Him Ears");
            }
            
        }
        
        eventType = (ushort)NetworkEvent.PlayerUpdateInfo;
    }

    public PlayerUpdateMessage(PlayerInfo info, string playerID) {
        eventType = (ushort)NetworkEvent.PlayerUpdateInfo;
        this.playerID = playerID;
        this.info = info;
    }
    public override byte[] toBuffer()
    {
        byte[] eventTypeBuffer = BitConverter.GetBytes(eventType);
        byte[] playerIDBuffer = Encoding.ASCII.GetBytes(playerID);
        byte[] playerIDLengthBuffer = BitConverter.GetBytes((ushort)playerIDBuffer.Length);
        byte[] zRotBuffer = BitConverter.GetBytes(info.zRot);
        byte[] xPosBuffer = BitConverter.GetBytes(info.position.x);
        byte[] yPosBuffer = BitConverter.GetBytes(info.position.y);
        byte[] speedBuffer = BitConverter.GetBytes(info.currentSpeed);
        byte[] colliderBuffer = BitConverter.GetBytes(info.collidable);
        byte[] scoreBuffer = BitConverter.GetBytes(info.currentScore);
        byte[] buffer = new byte[eventTypeBuffer.Length 
            + playerIDLengthBuffer.Length
            + playerIDBuffer.Length 
            + zRotBuffer.Length 
            + xPosBuffer.Length 
            + yPosBuffer.Length 
            + speedBuffer.Length
            + colliderBuffer.Length
            + scoreBuffer.Length];
        int currentIndex = 0;
        Buffer.BlockCopy(eventTypeBuffer, 0, buffer, currentIndex, eventTypeBuffer.Length);
        currentIndex += eventTypeBuffer.Length;
        Buffer.BlockCopy(playerIDLengthBuffer, 0, buffer, currentIndex, playerIDLengthBuffer.Length);
        currentIndex += playerIDLengthBuffer.Length;
        Buffer.BlockCopy(playerIDBuffer, 0, buffer, currentIndex, playerIDBuffer.Length);
        currentIndex += playerIDBuffer.Length;
        Buffer.BlockCopy(zRotBuffer, 0, buffer, currentIndex, zRotBuffer.Length);
        currentIndex += zRotBuffer.Length;
        Buffer.BlockCopy(xPosBuffer, 0, buffer, currentIndex, xPosBuffer.Length);
        currentIndex += xPosBuffer.Length;
        Buffer.BlockCopy(yPosBuffer, 0, buffer, currentIndex, yPosBuffer.Length);
        currentIndex += yPosBuffer.Length;
        Buffer.BlockCopy(speedBuffer, 0, buffer, currentIndex, speedBuffer.Length);
        currentIndex += speedBuffer.Length;
        Buffer.BlockCopy(colliderBuffer, 0, buffer, currentIndex, colliderBuffer.Length);
        currentIndex += colliderBuffer.Length;
        Buffer.BlockCopy(scoreBuffer, 0, buffer, currentIndex, scoreBuffer.Length);
        currentIndex += scoreBuffer.Length;
        return buffer;
    }
}

public class PlayerConnectedMessage : Message
{
    public string playerID;
    public PlayerConnectedMessage(byte[] buffer)
    {
        byte[] eventTypeBuffer = new byte[sizeof(ushort)];
        byte[] playerIDLengthBuffer = new byte[sizeof(ushort)];
        int currentIndex = 0;
        Buffer.BlockCopy(buffer, currentIndex, eventTypeBuffer, 0, eventTypeBuffer.Length);
        currentIndex += eventTypeBuffer.Length;
        Buffer.BlockCopy(buffer, currentIndex, playerIDLengthBuffer, 0, playerIDLengthBuffer.Length);
        currentIndex += playerIDLengthBuffer.Length;
        int playerIDBufferCount = BitConverter.ToUInt16(playerIDLengthBuffer, 0);
        byte[] playerIDBuffer = new byte[playerIDBufferCount]; //we know length of string now (in bytes)
        Buffer.BlockCopy(buffer, currentIndex, playerIDBuffer, 0, playerIDBuffer.Length);
        playerID = Encoding.ASCII.GetString(playerIDBuffer);
    }

    public PlayerConnectedMessage(string playerID) {
        eventType = (ushort)NetworkEvent.PlayerConnected;
        this.playerID = playerID;
    }
    public override byte[] toBuffer()
    {
        byte[] eventTypeBuffer = BitConverter.GetBytes(eventType);
        byte[] playerIDBuffer = Encoding.ASCII.GetBytes(playerID);
        byte[] playerIDLengthBuffer = BitConverter.GetBytes((ushort)playerIDBuffer.Length);
        byte[] buffer = new byte[eventTypeBuffer.Length + playerIDLengthBuffer.Length + playerIDBuffer.Length];
        int currentIndex = 0;
        Buffer.BlockCopy(eventTypeBuffer, 0, buffer, currentIndex, eventTypeBuffer.Length);
        currentIndex += eventTypeBuffer.Length;
        Buffer.BlockCopy(playerIDLengthBuffer, 0, buffer, currentIndex, playerIDLengthBuffer.Length);
        currentIndex += playerIDLengthBuffer.Length;
        Buffer.BlockCopy(playerIDBuffer, 0, buffer, currentIndex, playerIDBuffer.Length);
        currentIndex += playerIDBuffer.Length;
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

public class ObstacleGeneratedMessage : Message
{
    public uint itemID;
    public Vector2 itemPos;
    public /*enum*/ ushort itemType;

    public ObstacleGeneratedMessage(byte[] buffer)
    {
        eventType = (ushort)NetworkEvent.ObstacleGenerated;
        itemID = BitConverter.ToUInt32(buffer,2);
        itemPos = Vector2.zero;
        itemPos.x = BitConverter.ToSingle(buffer, 6);
        itemPos.y = BitConverter.ToSingle(buffer, 10);
        itemType = BitConverter.ToUInt16(buffer, 14);
    }

    public ObstacleGeneratedMessage(uint itemID, Vector2 itemPos, /*enum*/ ushort itemType)
    {
        eventType = (ushort)NetworkEvent.ObstacleGenerated;
        this.itemID = itemID;
        this.itemPos = itemPos;
        this.itemType = itemType;
    }

    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[16];
        byte[] eventTypeBuffer = BitConverter.GetBytes(eventType);
        byte[] itemIDBuffer = BitConverter.GetBytes(itemID);
        byte[] itemPosXBuffer = BitConverter.GetBytes(itemPos.x);
        byte[] itemPosYBuffer = BitConverter.GetBytes(itemPos.y);
        byte[] itemTypeBuffer = BitConverter.GetBytes(itemType);
        int currentIndex = 0;
        Buffer.BlockCopy(eventTypeBuffer, 0, buffer, currentIndex, eventTypeBuffer.Length);
        currentIndex += eventTypeBuffer.Length;
        Buffer.BlockCopy(itemIDBuffer, 0, buffer, currentIndex, itemIDBuffer.Length);
        currentIndex += itemIDBuffer.Length;
        Buffer.BlockCopy(itemPosXBuffer, 0, buffer, currentIndex, itemPosXBuffer.Length);
        currentIndex += itemPosXBuffer.Length;
        Buffer.BlockCopy(itemPosYBuffer, 0, buffer, currentIndex, itemPosYBuffer.Length);
        currentIndex += itemPosYBuffer.Length;
        Buffer.BlockCopy(itemTypeBuffer, 0, buffer, currentIndex, itemTypeBuffer.Length);
        currentIndex += itemTypeBuffer.Length;
        return buffer;
    }

}

public class PlayerFellBehindMessage : Message
{
    public string playerID;
    public PlayerFellBehindMessage(byte[] buffer)
    {
        byte[] eventTypeBuffer = new byte[sizeof(ushort)];
        byte[] playerIDLengthBuffer = new byte[sizeof(ushort)];
        int currentIndex = 0;
        Buffer.BlockCopy(buffer, currentIndex, eventTypeBuffer, 0, eventTypeBuffer.Length);
        currentIndex += eventTypeBuffer.Length;
        Buffer.BlockCopy(buffer, currentIndex, playerIDLengthBuffer, 0, playerIDLengthBuffer.Length);
        currentIndex += playerIDLengthBuffer.Length;
        int playerIDBufferCount = BitConverter.ToUInt16(playerIDLengthBuffer, 0);
        byte[] playerIDBuffer = new byte[playerIDBufferCount]; //we know length of string now (in bytes)
        Buffer.BlockCopy(buffer, currentIndex, playerIDBuffer, 0, playerIDBuffer.Length);
        playerID = Encoding.ASCII.GetString(playerIDBuffer);
    }

    public PlayerFellBehindMessage(string playerID)
    {
        eventType = (ushort)NetworkEvent.PlayerFellBehind;
        this.playerID = playerID;
    }
    public override byte[] toBuffer()
    {
        byte[] eventTypeBuffer = BitConverter.GetBytes(eventType);
        byte[] playerIDBuffer = Encoding.ASCII.GetBytes(playerID);
        byte[] playerIDLengthBuffer = BitConverter.GetBytes((ushort)playerIDBuffer.Length);
        byte[] buffer = new byte[eventTypeBuffer.Length + playerIDLengthBuffer.Length + playerIDBuffer.Length];
        int currentIndex = 0;
        Buffer.BlockCopy(eventTypeBuffer, 0, buffer, currentIndex, eventTypeBuffer.Length);
        currentIndex += eventTypeBuffer.Length;
        Buffer.BlockCopy(playerIDLengthBuffer, 0, buffer, currentIndex, playerIDLengthBuffer.Length);
        currentIndex += playerIDLengthBuffer.Length;
        Buffer.BlockCopy(playerIDBuffer, 0, buffer, currentIndex, playerIDBuffer.Length);
        currentIndex += playerIDBuffer.Length;
        return buffer;
    }
}

public class ObstacleModifiedMessage : Message
{
    public uint obstacleID;
    public string playerID;
    public ObstacleModifiedMessage(byte[] buffer)
    {
        int currentByteIndex = 0;
        eventType = BitConverter.ToUInt16(buffer, currentByteIndex);
        currentByteIndex += sizeof(ushort);
        int playerIDCount = BitConverter.ToUInt16(buffer, currentByteIndex);
        currentByteIndex += sizeof(ushort);
        playerID = "";
        for (int i = 0; i < playerIDCount; i++)
        {
            playerID += BitConverter.ToChar(buffer, currentByteIndex);
            currentByteIndex += sizeof(char);
        }
        obstacleID = BitConverter.ToUInt32(buffer, currentByteIndex);
        currentByteIndex += sizeof(uint);
    }

    public ObstacleModifiedMessage(string playerID, uint obstacleID)
    {
        eventType = (ushort)NetworkEvent.ObstacleModified;
        this.playerID = playerID;
        this.obstacleID = obstacleID;
    }
    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[4 + (playerID.Length * sizeof(char)) + sizeof(uint)];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        uint playerIDLength = (ushort)playerID.Length;
        Buffer.BlockCopy(BitConverter.GetBytes(playerIDLength), 0, buffer, 2, 2);
        for (int i = 0; i < playerID.Length; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(playerID.ToCharArray()[i]), 0, buffer, 4 + i * sizeof(char), sizeof(char));
        }
        Buffer.BlockCopy(BitConverter.GetBytes(obstacleID), 0, buffer, buffer.Length - sizeof(uint), sizeof(uint));
        return buffer;
    }

}

//TODO
public class ObstacleMovedMessage : Message
{
    public uint obstacleID;
    public string playerID;
    public Vector3 offset; //how much did it move
    public float timeToOffset; //time to travel for anim purposes


    public ObstacleMovedMessage(byte[] buffer)
    {
        int currentByteIndex = 0;
        eventType = BitConverter.ToUInt16(buffer, currentByteIndex);
        currentByteIndex += sizeof(ushort);
        int playerIDCount = BitConverter.ToUInt16(buffer, currentByteIndex);
        currentByteIndex += sizeof(ushort);
        playerID = "";
        for (int i = 0; i < playerIDCount; i++)
        {
            playerID += BitConverter.ToChar(buffer, currentByteIndex);
            currentByteIndex += sizeof(char);
        }
        obstacleID = BitConverter.ToUInt32(buffer, currentByteIndex);
        currentByteIndex += sizeof(uint);
        offset = Vector3.zero;
        offset.x = BitConverter.ToSingle(buffer, currentByteIndex);
        currentByteIndex += sizeof(float);

    }

    public ObstacleMovedMessage(string playerID, uint obstacleID)
    {
        eventType = (ushort)NetworkEvent.ObstacleModified;
        this.playerID = playerID;
        this.obstacleID = obstacleID;
    }
    public override byte[] toBuffer()
    {
        byte[] buffer = new byte[4 + (playerID.Length * sizeof(char)) + sizeof(uint)];
        Buffer.BlockCopy(BitConverter.GetBytes(eventType), 0, buffer, 0, 2);
        uint playerIDLength = (ushort)playerID.Length;
        Buffer.BlockCopy(BitConverter.GetBytes(playerIDLength), 0, buffer, 2, 2);
        for (int i = 0; i < playerID.Length; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(playerID.ToCharArray()[i]), 0, buffer, 4 + i * sizeof(char), sizeof(char));
        }
        Buffer.BlockCopy(BitConverter.GetBytes(obstacleID), 0, buffer, buffer.Length - sizeof(uint), sizeof(uint));
        return buffer;
    }
}

public class PlayerWonMessage : Message
{
    public string playerID;
    public PlayerWonMessage(byte[] buffer)
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

    public PlayerWonMessage(string playerID)
    {
        eventType = (ushort)NetworkEvent.PlayerWonRace;
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