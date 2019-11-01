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
        MovePlayer(Time.deltaTime);
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, 0);
        playerInfo.position = pos;
    }

    public void MovePlayer(float deltaTime)
    {
        transform.position = playerInfo.position;
        playerInfo.currentSpeed = Mathf.Clamp(playerInfo.currentSpeed + (acceleration * deltaTime), 0, MaxSpeed);
        playerInfo.position += new Vector2(transform.up.x, transform.up.y) * playerInfo.currentSpeed * deltaTime;
        transform.rotation = Quaternion.Euler(0.0f,0.0f, playerInfo.zRot);
        transform.position = Vector3.Lerp(transform.position, playerInfo.position, 0.75f);
        playerInfo.position = transform.position;
    }

    public void LookForObstacles()
    {
        for(int i = 0; i < Obstacle.getAllObstacleCount(); i++)
        {
            if(CheckIfClose(playerInfo.position,Obstacle.m_AllObstacles[i].transform.position))
            {
                Obstacle.m_AllObstacles[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                if(Input.GetKey(KeyCode.E))
                {
                    //Interact with the obstacle
                }
            }
        }
    }

    public bool CheckIfClose(Vector2 playerPos, Vector2 obstPos)
    {
        if((obstPos - playerPos).magnitude < 3.0f)
        {
            Debug.Log("Near obstacle");
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerator SpinPlayerDuration(float duration)
    {
        float time = 0.0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            transform.Rotate(0, 0, 20, Space.Self);
            //m_SpriteRenderer.transform.Rotate(0, 0, 180, Space.Self);
            Debug.Log("did rotatino");
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void StartSpin()
    {
        StartCoroutine(SpinPlayerDuration(1.0f));
        //m_SpriteRenderer.transform.Rotate(0, 0, 90, Space.Self);

    }

}
