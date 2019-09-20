﻿using System.Collections;
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
    public PlayerMove move;
}

public enum PlayerMove //possible actions (limited to what buttosn the player could hit
{
    TURNLEFT,
    TURNRIGHT,
    ATTACK,
    OLLIE,
    NONE
    //add more, clean up options
}
/*public struct PlayerInteractInfo
{
    public PlayerMove move;
    //public int comboCount //used for acceleration, still need to work on this
}
*/

//probably two seperate player scripts, ClientPlayer and NetworkPlayer
public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    [SerializeField]
    protected float startSpeed;

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

    protected Rigidbody2D m_Rigidbody;

    protected SpriteRenderer m_SpriteRenderer;

    //ADD VARIABLES IF NEEDED FOR ACCELERATION
    protected float MaxSpeed;

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_Rigidbody = player.GetComponent<Rigidbody2D>();
        m_SpriteRenderer = player.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    //Needed to be public in order to get the base.Update(); to work. Not sure if there's another way. https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/base
    public virtual void FixedUpdate()
    {

        if (posInfo.move == PlayerMove.TURNLEFT)
        {
            transform.Rotate(new Vector3(0, 0, -1)* Time.deltaTime * posInfo.currentSpeed * 3.0f, Space.World);
        }
        if (posInfo.move == PlayerMove.TURNRIGHT)
        {
            transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * posInfo.currentSpeed * 3.0f, Space.World);
        }
        if (posInfo.move == PlayerMove.OLLIE)
        {
            m_SpriteRenderer.color = Color.red;
        }
        //update positions based on the input of the player
        //m_Rigidbody.velocity = transform.up * posInfo.currentSpeed; Rigidbody will end up as a part of the player once base.Update() is working
        //transform.Rotate(new Vector3(0,0,posInfo.zRot).normalized) * Time.deltaTime * posInfo.currentSpeed, Space.World); Still need to test this some more.
        //update the speed of the player
        if (posInfo.move == PlayerMove.NONE)
        {
            transform.rotation = Quaternion.identity;
            posInfo.zRot = 0.0f;
            posInfo.currentSpeed += speedIncrease * accelMod;
            m_SpriteRenderer.color = Color.white;
        }

        m_Rigidbody.velocity = transform.up * posInfo.currentSpeed;
        posInfo.position = transform.position;

    }
    public float getMaxSpeed()
    {
        MaxSpeed = (posInfo.currentScore * speedMod) + startSpeed;
        return MaxSpeed;
    }
}
