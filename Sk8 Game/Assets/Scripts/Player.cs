﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains All The Shared Data and functions between ClientPlayer and NetworkedPlayer
 * Specific functions can be added to ClientPlayer and NetworkedPlayer classes as well.
 */
public class PlayerInfo //sent from server to 
{
    [SerializeField]
    public Vector2 position = Vector2.zero;
    [SerializeField]
    public float zRot = 0.0f;
    [SerializeField]
    public float currentSpeed = 0.0f;
    [SerializeField]
    public int currentScore = 0;
    [SerializeField]
    public bool collidable = false;
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
    public PlayerInfo playerInfo = new PlayerInfo();

    protected Rigidbody2D m_Rigidbody;

    protected SpriteRenderer m_SpriteRenderer;
    public Sprite[] sprites;


    public float MaxSpeed
    {
        get
        {
            return (playerInfo.currentScore * speedMod) + startSpeed;
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        playerInfo.collidable = true;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        //m_SpriteRenderer.sprite = sprites[1];
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
        
        BackDraft();
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

    public bool CheckIfClose(Vector2 playerPos, Vector2 obstPos)
    {
        if((obstPos - playerPos).magnitude < 3.0f)
        {
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
        //the spin should be visual
        //StartCoroutine(SpinPlayerDuration(1.0f));
        //m_SpriteRenderer.transform.Rotate(0, 0, 90, Space.Self);

    }

    public void BackDraft()
    {
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            if (playerInfo.position.y <= (GameManager.Instance.m_Players[i].transform.position.y - 10))
            {
                playerInfo.currentSpeed *= 1.6f * Time.deltaTime;
            }
            else
            {
            }
        }
    }

    public IEnumerator Dodge(float duration)
    {
        float time = 0.0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            playerInfo.collidable = false;
            m_SpriteRenderer.sprite = sprites[1];
            yield return 0;
        }
        m_SpriteRenderer.sprite = sprites[0];
        playerInfo.collidable = true;
    }
}
