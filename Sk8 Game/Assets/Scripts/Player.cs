using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains All The Shared Data and functions between ClientPlayer and NetworkedPlayer
 * Specific functions can be added to ClientPlayer and NetworkedPlayer classes as well.
 */
public struct PlayerPosInfo //sent from server to 
{
    public Vector3 pos;
    public float zRot;
    public float currentSpeed;
}

public enum PlayerMove //possible actions (limited to what buttosn the player could hit
{
    TURNLEFT,
    TURNRIGHT,
    ATTACK,
    OLLIE
    //add more, clean up options
}
public struct PlayerInteractInfo
{
    public PlayerMove move;
}

//probably two seperate player scripts, ClientPlayer and NetworkPlayer
public class Player : MonoBehaviour
{
    
    //ADD VARIABLES IF NEEDED FOR ACCELERATION
    float MaxSpeed { get
        {
            return 0.0f; //todo
        }
    }
    float mCurrentSpeed = 0.0f;
    int mCurrentScore = 0; //used to calculate max speed
    float mTurnDeg; //turn degree from the up axis, so negative is left, positive is right
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //check if client player turns
        
    }
}
