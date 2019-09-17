using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : Player
{



    PlayerPosInfo NetworkedPosInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*update posInfo struct
        data needs to be recieved from the server first
        planned code will be as follows
        NetworkedPosInfo.currentSpeed = passedSpeed;
        NetworkPos.Info.zRot = passedZRot;
        NetworkPosInfo.position = passedRot;
        This will be done for each player
        
        base.Update(); //after all information is updated, use the player update
        */
    }
}
