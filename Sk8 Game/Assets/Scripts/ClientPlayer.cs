using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note: More than one client player can exist, make sure to not hardcode in controls except for early testing
 */
public class ClientPlayer : Player
{

    private float leftTime;
    private float rightTime;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get keypress, send to server
        //update posInfo based on input

        if (Input.GetKeyDown("LeftArrow"))
        {
            leftTime = Time.time;
        }
        if (Input.GetKeyDown("RightArrow"))
        {
            rightTime = Time.time;
        }
        if (Input.GetKeyUp("LeftArrow"))
        {

        }
        if (Input.GetKeyUp("RightArrow"))
        {

        }
        //send new info to the server after player update function is called
    }
    
}
