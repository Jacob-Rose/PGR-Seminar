using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains All The Shared Data and functions between ClientPlayer and NetworkedPlayer
 * Specific functions can be added to ClientPlayer and NetworkedPlayer classes as well.
 */
public struct PlayerInfo //sent from server to 
{
    [SerializeField]
    public Vector2 position;
    [SerializeField]
    public float zRot;
    [SerializeField]
    public float currentSpeed;
    [SerializeField]
    public int currentScore;
    [SerializeField]
    public PlayerMove move;
    [SerializeField]
    public int id;
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

public class Player : MonoBehaviour, Listener
{
    [SerializeField]
    protected float startSpeed;

    [SerializeField]
    protected float speedDecreaseAmount;

    [SerializeField]
    protected float speedMod; //used for max speed calculation

    [SerializeField]
    protected float acceleration; //modifier for the acceleration when the player speed rises in Update

    [SerializeField]
    public PlayerInfo playerInfo;

    protected Rigidbody2D m_Rigidbody;

    protected SpriteRenderer m_SpriteRenderer;

    protected float MaxSpeed
    {
        get
        {
            return (playerInfo.currentScore * speedMod) + startSpeed;
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        Toolbox.Instance.addPlayer(this);
    }

    public PlayerInfo GetPlayerInfo()
    {
        return playerInfo;
    }

    // Update is called once per frame
    //Needed to be public in order to get the base.Update(); to work. Not sure if there's another way. https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/base
    public virtual void Update()
    {
        if (!Toolbox.Instance.HasGameStarted)
            return;
        MovePlayer(Time.deltaTime);

    }

    public void MovePlayer(float deltaTime)
    {
        if (playerInfo.move == PlayerMove.OLLIE)
        {
            m_SpriteRenderer.color = Color.red;
        }
        //update positions based on the input of the player
        //m_Rigidbody.velocity = transform.up * posInfo.currentSpeed; Rigidbody will end up as a part of the player once base.Update() is working
        //transform.Rotate(new Vector3(0,0,posInfo.zRot).normalized) * Time.deltaTime * posInfo.currentSpeed, Space.World); Still need to test this some more.
        //update the speed of the player
        if (playerInfo.move == PlayerMove.NONE)
        {
            playerInfo.currentSpeed += acceleration;
            m_SpriteRenderer.color = Color.white;
        }

        m_Rigidbody.velocity = transform.up * playerInfo.currentSpeed;
        playerInfo.position = transform.position;
        transform.rotation = Quaternion.Euler(0.0f,0.0f, playerInfo.zRot);
    }

    public void OnListenerCall()
    {
        
    }
}
