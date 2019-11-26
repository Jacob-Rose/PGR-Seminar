using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    public bool collidable;
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

public abstract class Player : MonoBehaviour
{
    [SerializeField]
    protected float startSpeed;

    [SerializeField]
    protected float speedMod; //used for max speed calculation

    [SerializeField]
    protected float timeToMaxSpeed; //modifier for the acceleration when the player speed rises in Update

    [SerializeField]
    public PlayerInfo playerInfo;

    protected Rigidbody2D m_Rigidbody;
    protected SpriteRenderer m_SpriteRenderer;

    public Vector2 m_DraftBounds = new Vector2(2.5f, 10.0f);
    public float m_BackDraftMultiplier = 1.1f;

    public bool m_IsDodging = false;
    public bool m_IsSpinning = false;

    public int m_SpinCount = 2;
    public float m_SpinDuration = 1.0f;


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
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //m_SpriteRenderer.sprite = sprites[1];
        playerInfo.currentSpeed = MaxSpeed;
        playerInfo.position = transform.position;
        DontDestroyOnLoad(this);
    }

    public abstract string GetUsername();
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public virtual void Update()
    {
        if (!GameManager.Instance.HasGameStarted)
            return;
        MovePlayer(Time.deltaTime);
        CheckBackDraft(Time.deltaTime);
        CheckDodgeSprite(Time.deltaTime);
    }

    private void CheckDodgeSprite(float deltaTime)
    {
        GameObject spriteChild = transform.GetChild(0).gameObject;
        SpriteRenderer sr = spriteChild.GetComponent<SpriteRenderer>();
        if (playerInfo.collidable)
        {
            sr.transform.localScale = Vector3.Lerp(sr.transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.2f);
        }
        else
        {
            sr.transform.localScale = Vector3.Lerp(sr.transform.localScale, new Vector3(1.3f, 1.3f, 1.3f), 0.2f); //could make a variable but who cares really
        }
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, 0);
        playerInfo.position = pos;
    }

    public void MovePlayer(float deltaTime)
    {
        transform.position = playerInfo.position;
        playerInfo.currentSpeed = Mathf.Lerp(playerInfo.currentSpeed, MaxSpeed, (deltaTime / timeToMaxSpeed));
        playerInfo.position += new Vector2(transform.up.x, transform.up.y) * playerInfo.currentSpeed * deltaTime;
        transform.rotation = Quaternion.Euler(0.0f,0.0f, playerInfo.zRot);
        transform.position = Vector3.Lerp(transform.position, playerInfo.position, 0.6f); //in case the update is off from current position
        playerInfo.position = transform.position;
    }


    public IEnumerator SpinPlayerDuration(float duration)
    {
        m_IsSpinning = true;
        float time = 0.0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float rotAmount = 360.0f * ((time / duration) * m_SpinCount); //two full spins before done
            m_SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rotAmount);
            yield return 0;
        }
        m_SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        m_IsSpinning = false;

    }
    public void StartSpin()
    {
        if(!m_IsSpinning)
        {
            StartCoroutine(SpinPlayerDuration(m_SpinDuration));
        }
    }

    public void CheckBackDraft(float deltaTime)
    {
        bool drafting = false;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            Player oPlayer = GameManager.Instance.m_Players[i];
            if(oPlayer != this)
            {
                Bounds draftBounds = new Bounds(oPlayer.playerInfo.position - new Vector2(0.0f, m_DraftBounds.y * 0.5f), m_DraftBounds);
                if (draftBounds.Contains(playerInfo.position))
                {
                    playerInfo.currentSpeed = Mathf.Lerp(playerInfo.currentSpeed, MaxSpeed * m_BackDraftMultiplier, deltaTime);
                    drafting = true;
                }
            }
        }
        if(this is ClientPlayer)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                PostProcessVolume v = cam.GetComponent<PostProcessVolume>();
                if (v != null)
                {
                    ChromaticAberration ca;
                    v.profile.TryGetSettings(out ca);
                    if (drafting)
                    {
                        ca.intensity.value = Mathf.Lerp(ca.intensity.value, 1, 0.2f);
                    }
                    else
                    {
                        ca.intensity.value = Mathf.Lerp(ca.intensity.value, 0, 0.2f);
                    }
                }
            }
        }
    }
}
