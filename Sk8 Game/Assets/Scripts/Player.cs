using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains All The Shared Data and functions between ClientPlayer and NetworkedPlayer
 * Specific functions can be added to ClientPlayer and NetworkedPlayer classes as well.
 */
public struct PlayerInfo //sent from server to 
{
    public Vector3 position;
    public float zRot;
    public float currentSpeed;
    public int currentScore;
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
    protected float m_Speed; //THIS IS FOR TESTING ONLY, WILL BE REPLACED LATER 
    //set how much a rotation changes the player
    [SerializeField]
    protected float zRotAmount;

    [SerializeField]
    protected float speedDecreaseAmount;

    [SerializeField]
    protected float speedMod; //used for max speed calculation

    [SerializeField]
    protected float accelMod; //modifier for the acceleration when the player speed rises in Update

    [SerializeField]
    protected float speedIncrease; //Amount speed goes up by each time update is called, accelMod directly influences the value, allow the speedIncrease to amp up as time goes on

    protected PlayerInfo posInfo;

    //ADD VARIABLES IF NEEDED FOR ACCELERATION
    protected float MaxSpeed { get
        {
            return 0.0f; //todo
            //maxSpeed += mCurrentScore * speedMod; //speedMod can be set to a value we think is acceptable in the editor
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //Needed to be public in order to get the base.Update(); to work. Not sure if there's another way. https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/base
    public virtual void Update()
    {


        //update positions based on the input of the player
        //m_Rigidbody.velocity = transform.up * posInfo.currentSpeed; Rigidbody will end up as a part of the player once base.Update() is working
        //transform.Rotate(new Vector3(0,0,posInfo.zRot).normalized) * Time.deltaTime * posInfo.currentSpeed, Space.World); Still need to test this some more.
        //update the speed of the player
        if (posInfo.currentSpeed <= MaxSpeed && posInfo.zRot == 0) //speed only increases if the player is not turning, functionality for tricks to come
        {
            //mCurrentSpeed += speedIncrease * accelMod * comboCount
        }

        posInfo.position = transform.position;

    }
}
