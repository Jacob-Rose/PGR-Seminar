using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note: More than one client player can exist, make sure to not hardcode in controls except for early testing
 */
public class ClientPlayer : Player
{
    //set how much a rotation changes the player
    [SerializeField]
    private float zRotAmount;

    [SerializeField]
    private float speedDecreaseAmount;

    PlayerPosInfo clientPosInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get keypress, send to server
        //update posInfo based on input

        //instead of calculating how long key is held, it just adds a constant amount each time. This can change, just still working on timers.
        if (Input.GetKeyDown("LeftArrow"))
        {
            clientPosInfo.zRot -= zRotAmount;
            clientPosInfo.currentSpeed -= speedDecreaseAmount;
        }
        if (Input.GetKeyDown("RightArrow"))
        {
            clientPosInfo.zRot += zRotAmount;
            clientPosInfo.currentSpeed -= speedDecreaseAmount;
        }
        if (Input.GetKeyUp("LeftArrow"))
        {
            clientPosInfo.zRot = 0.0f;
        }
        if (Input.GetKeyUp("RightArrow"))
        {
            clientPosInfo.zRot = 0.0f;
        }
        //send new info to the server after player update function is called
        base.Update();


    }
    
}
