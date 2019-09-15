using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : Player
{
    PlayerInteractInfo serverPlayerInteract;
    PlayerPosInfo serverPlayerPosInfo;

    private Vector3 playerPos;
    private float playerSpeed;
    private float playerZRot;

    private PlayerMove moveType;

    // Start is called before the first frame update
    void Start()
    {
        serverPlayerInteract = new PlayerInteractInfo();
        serverPlayerPosInfo = new PlayerPosInfo();
        
    }

    // Update is called once per frame
    void Update()
    {
        serverPlayerInteract.move = moveType;
        serverPlayerPosInfo.pos = playerPos;
        serverPlayerPosInfo.currentSpeed = playerSpeed;
        serverPlayerPosInfo.zRot = playerZRot;
        if (moveType == PlayerMove.ATTACK)
        {
            //AttackEvent(playerPos, )//still need a way to get another player position
        }
        if (moveType == PlayerMove.OLLIE)
        {
            TrickEvent(playerPos, "Ollie"); //trick name is hard coded for now
        }
        if (moveType == PlayerMove.TURNLEFT)
        {
            MoveLeftEvent(playerPos, playerSpeed);
        }
        if (moveType == PlayerMove.TURNRIGHT)
        {
            MoveRightEvent(playerPos, playerSpeed);
        }
        //send the data to the client

    }

    void MoveLeftEvent(Vector3 pos, float speed) //not sure if values passed in are needed
    {
        playerPos = pos;
        playerSpeed = speed;
        playerPos -= new Vector3(0f, 2f, 0f);
        playerSpeed -= 0.75f;
    }

    void MoveRightEvent(Vector3 pos, float speed)
    {
        playerPos = pos;
        playerSpeed = speed;
        playerPos += new Vector3(0f, 2f, 0f); //update position
        playerSpeed -= 0.75f; //subtract player speed
    }

    void TrickEvent(Vector3 pos, string trickName)
    {
        //todo, string will tell the game what trick to perform
    }

    void AttackEvent(Vector3 playerPos, Vector3 player2Pos, float range)
    {
        //todo
    }
}
