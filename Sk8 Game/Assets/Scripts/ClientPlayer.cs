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
        
        
        //get keypress, send to server



        /*if (playerInteract.move == PlayerMove.ATTACK)
        {
            //AttackEvent(playerPos, )//still need a way to get another player position
        }
        if (playerInteract.move == PlayerMove.OLLIE)
        {
            TrickEvent(); //trick name is hard coded for now
        }
        if (playerInteract.move == PlayerMove.TURNLEFT)
        {
            MoveLeftEvent();
        }
        if (playerInteract.move == PlayerMove.TURNRIGHT)
        {
            MoveRightEvent();
        }*/

    }
    /*void MoveLeftEvent() //not sure if values passed in are needed
    {
        
        posInfo.pos -= new Vector3(0f, 2f, 0f); //value can be passed from server if needed
    }

    void MoveRightEvent()
    {
        
        posInfo.pos += new Vector3(0f, 2f, 0f);
    }

    void TrickEvent()
    {
        //todo
    }

    void AttackEvent()
    {
        //todo
    }*/
}
