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

public class Player : MonoBehaviour
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

    private bool lastFrameGameStarted = false;

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
        playerInfo.currentSpeed = MaxSpeed;
        playerInfo.position = transform.position;
        DontDestroyOnLoad(this);
    }

    public PlayerInfo GetPlayerInfo()
    {
        return playerInfo;
    }

    public virtual void Update()
    {
        if (!GameManager.Instance.HasGameStarted)
            return;
        if(!lastFrameGameStarted)
        {
            lastFrameGameStarted = true;
            playerInfo.position = transform.position;
        }
        MovePlayer(Time.deltaTime);

    }

    public void LateUpdate()
    {
        if (!GameManager.Instance.HasGameStarted)
            return;
        playerInfo.position = transform.position;
    }

    public void MovePlayer(float deltaTime)
    {
        transform.position = playerInfo.position;
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

        playerInfo.position += new Vector2(transform.up.x, transform.up.y) * playerInfo.currentSpeed * deltaTime;
        transform.rotation = Quaternion.Euler(0.0f,0.0f, playerInfo.zRot);
        transform.position = Vector3.Lerp(transform.position, playerInfo.position, 0.75f);
        playerInfo.position = transform.position;
    }
}
