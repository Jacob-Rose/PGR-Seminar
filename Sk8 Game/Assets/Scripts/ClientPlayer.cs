using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note: More than one client player can exist, make sure to not hardcode in controls except for early testing
 */
public class ClientPlayer : Player
{
    PlayerInteractInfo playerInteract;
    PlayerPosInfo posInfo;

    private Vector3 playerPos;
    private float playerSpeed;
    private float playerZRot;

    private PlayerMove moveType;

    // Start is called before the first frame update
    void Start()
    {
        playerInteract = new PlayerInteractInfo();
        posInfo = new PlayerPosInfo();
    }

    // Update is called once per frame
    void Update()
    {
        //upon data recieved, send a bool to the server
        playerInteract.move = moveType;
        posInfo.pos = playerPos;
        posInfo.currentSpeed = playerSpeed;
        posInfo.zRot = playerZRot;
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
            MoveLeftEvent(playerPos);
        }
        if (moveType == PlayerMove.TURNRIGHT)
        {
            MoveRightEvent(playerPos);
        }

    }
    void MoveLeftEvent(Vector3 pos) //not sure if values passed in are needed
    {
        playerPos = pos;
        playerPos -= new Vector3(0f, 2f, 0f); //value can be passed from server if needed
    }

    void MoveRightEvent(Vector3 pos)
    {
        playerPos = pos;
        playerPos += new Vector3(0f, 2f, 0f);
    }

    void TrickEvent(Vector3 pos, string trickName)
    {
        //todo
    }

    void AttackEvent(Vector3 playerPos, Vector3 player2Pos, float range)
    {
        //todo
    }
}
