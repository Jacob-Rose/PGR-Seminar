using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note: More than one client player can exist, make sure to not hardcode in controls except for early testing
 */
public class ClientPlayer : Player
{

    public float zRotAmount = 10.0f;
    // Start is called before the first frame update
    public override void Start()
    {

        playerInfo.currentSpeed = MaxSpeed;
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        //get keypress, send to server
        //update posInfo based on input

        //instead of calculating how long key is held, it just adds a constant amount each time. This can change, just still working on timers.
        HandleInput(Time.deltaTime);
        base.Update();
    }

    public void HandleInput(float deltaTime)
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerInfo.move = PlayerMove.TURNLEFT;
            playerInfo.zRot -= zRotAmount * deltaTime;
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            playerInfo.move = PlayerMove.TURNRIGHT;
            playerInfo.zRot += zRotAmount * deltaTime;
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            playerInfo.move = PlayerMove.OLLIE;
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime;
        }
        else
        {
            playerInfo.move = PlayerMove.NONE;
            playerInfo.zRot = Mathf.Lerp(playerInfo.zRot, 0, 0.1f);
        }
    }
    
}
