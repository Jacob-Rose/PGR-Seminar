using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains All The Shared Data and functions between ClientPlayer and NetworkedPlayer
 * Specific functions can be added to ClientPlayer and NetworkedPlayer classes as well.
 */
public struct PlayerPosInfo //sent from server to 
{
    public Vector3 position;
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
    //public int comboCount //used for acceleration, still need to work on this
}

//probably two seperate player scripts, ClientPlayer and NetworkPlayer
public class Player : MonoBehaviour
{
    [SerializeField]
    private float speedMod; //used for max speed calculation

    [SerializeField]
    private float accelMod; //modifier for the acceleration when the player speed rises in Update

    [SerializeField]
    private float speedIncrease; //Amount speed goes up by each time update is called, accelMod directly influences the value, allow the speedIncrease to amp up as time goes on

    PlayerPosInfo posInfo;

    //ADD VARIABLES IF NEEDED FOR ACCELERATION
    float MaxSpeed { get
        {
            return 0.0f; //todo
            //maxSpeed += mCurrentScore * speedMod; //speedMod can be set to a value we think is acceptable in the editor
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
    //Needed to be public in order to get the base.Update(); to work. Not sure if there's another way. https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/base
    public virtual void Update()
    {
        //update positions based on the input of the player
        posInfo.position = transform.rotation.z * posInfo.zRot * Vector3.forward;
        //currentSpeed = passedInSpeed //this is from the server, I think

        //update the speed of the player
        if (mCurrentSpeed <= MaxSpeed)
        {
            //mCurrentSpeed += speedIncrease * accelMod * comboCount
        }

    }
}
