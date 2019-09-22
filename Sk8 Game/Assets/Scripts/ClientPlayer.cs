using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note: More than one client player can exist, make sure to not hardcode in controls except for early testing
 */
public class ClientPlayer : Player
{

    // Start is called before the first frame update
    public override void Start()
    {

        playerInfo.currentSpeed = getMaxSpeed();
        base.Start();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        //get keypress, send to server
        //update posInfo based on input

        //instead of calculating how long key is held, it just adds a constant amount each time. This can change, just still working on timers.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerInfo.move = PlayerMove.TURNLEFT;
            playerInfo.zRot -= zRotAmount;
            playerInfo.currentSpeed -= speedDecreaseAmount;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            playerInfo.move = PlayerMove.TURNRIGHT;
            playerInfo.zRot += zRotAmount;
            playerInfo.currentSpeed -= speedDecreaseAmount;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            playerInfo.move = PlayerMove.OLLIE;
            playerInfo.currentSpeed -= speedDecreaseAmount;
        }
        else
        {
            playerInfo.move = PlayerMove.NONE;
        }
        base.FixedUpdate();
    }
    
}
