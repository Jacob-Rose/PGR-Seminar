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

        posInfo.currentSpeed = getMaxSpeed();
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
            posInfo.move = PlayerMove.TURNLEFT;
            posInfo.zRot -= zRotAmount;
            posInfo.currentSpeed -= speedDecreaseAmount;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            posInfo.move = PlayerMove.TURNRIGHT;
            posInfo.zRot += zRotAmount;
            posInfo.currentSpeed -= speedDecreaseAmount;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            posInfo.move = PlayerMove.OLLIE;
            posInfo.currentSpeed -= speedDecreaseAmount;
        }
        else
        {
            posInfo.move = PlayerMove.NONE;
        }
        base.FixedUpdate();


    }
    
}
