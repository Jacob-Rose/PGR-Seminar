using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/*
 * Contains All The Shared Data and functions between ClientPlayer and NetworkedPlayer
 * Specific functions can be added to ClientPlayer and NetworkedPlayer classes as well.
 */
public struct PlayerInfo //the struct that is synced between networked players
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
    [SerializeField]
    public bool attacking;
    [SerializeField]
    public float stamina;
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
    protected float m_TimeToMaxSpeed; //modifier for the acceleration when the player speed rises in Update
    [SerializeField]
    protected float m_MaxStamina = 5.0f;
    [SerializeField]
    protected float m_StaminaRefillPerSecond = 1.0f;

    public float m_DodgeTimeInAir = 1.5f;

    [SerializeField]
    public PlayerInfo m_PlayerInfo;

    protected Rigidbody2D m_Rigidbody;
    protected SpriteRenderer m_SpriteRenderer;
    protected AudioSource m_AudioSource;

    public Vector2 m_DraftBounds = new Vector2(2.5f, 10.0f);
    public float m_BackDraftMultiplier = 1.1f;

    public bool m_IsDodging = false;
    public bool m_IsSpinning = false;
    public bool m_IsAttacking = false;
    public int m_SpinCount = 2;
    public float m_SpinDuration = 1.0f;
    public float m_AttackDuration = 1.0f;

    public AudioClip ollieStartClip;
    public AudioClip ollieEndClip;
    public AudioClip rollingClip;


    public float MaxSpeed
    {
        get
        {
            return (m_PlayerInfo.currentScore * speedMod) + startSpeed;
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_AudioSource = GetComponent<AudioSource>();

        m_PlayerInfo.collidable = true;
        m_PlayerInfo.currentSpeed = MaxSpeed;
        m_PlayerInfo.position = transform.position;

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
        CheckAttackSprite(Time.deltaTime);
        CheckPlayerSound();
    }

    private void CheckDodgeSprite(float deltaTime)
    {
        GameObject spriteChild = transform.GetChild(0).gameObject;
        SpriteRenderer sr = spriteChild.GetComponent<SpriteRenderer>();
        if (m_PlayerInfo.collidable)
        {
            sr.transform.localScale = Vector3.Lerp(sr.transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.2f);
        }
        else
        {
            sr.transform.localScale = Vector3.Lerp(sr.transform.localScale, new Vector3(1.3f, 1.3f, 1.3f), 0.2f); //could make a variable but who cares really
        }
    }

    public void CheckPlayerSound()
    {
        if (m_AudioSource != null)
        {
            if (!m_PlayerInfo.collidable)
            {
                TryPlayOllieStartSound();
            }
            else
            {
                TryPlayRollingSound();
            }

            m_AudioSource.volume = 0.8f / GameManager.Instance.m_Players.Count;
        }
    }

    public void TryPlayOllieStartSound()
    {
        if (m_AudioSource.clip != ollieStartClip)
        {
            m_AudioSource.Stop();
            m_AudioSource.clip = ollieStartClip;
            m_AudioSource.loop = false;
            m_AudioSource.time = 0;
            m_AudioSource.Play();
            Invoke("TryPlayOllieEndSound", m_DodgeTimeInAir * 0.95f);
        }
    }

    public void TryPlayOllieEndSound()
    {
        if (m_AudioSource.clip != ollieEndClip)
        {
            m_AudioSource.Stop();
            m_AudioSource.clip = ollieEndClip;
            m_AudioSource.loop = false;
            m_AudioSource.time = 0;
            m_AudioSource.Play();
        }
    }

    public void TryPlayRollingSound()
    {
        if (m_AudioSource.clip != rollingClip)
        {
            m_AudioSource.Stop();
            m_AudioSource.clip = rollingClip;
            m_AudioSource.time = 0.0f;
            m_AudioSource.loop = true;
            m_AudioSource.Play();
        }
        else
        {
            m_AudioSource.pitch = (m_PlayerInfo.currentSpeed / MaxSpeed) * Mathf.Clamp(1.5f/ Mathf.Clamp(Mathf.Abs(m_PlayerInfo.zRot / 30), 0, 3), 0.5f, 1.5f);
        }
    }

    private void CheckAttackSprite(float deltaTime)
    {
        GameObject spriteChild = transform.GetChild(0).gameObject;
        SpriteRenderer sr = spriteChild.GetComponent<SpriteRenderer>();

        if (m_PlayerInfo.attacking)
        {
            sr.sprite = Resources.Load<Sprite>("Sprites/sk8rboiBigPunch2");
            sr.flipX = true;
        }
        else
        {
            sr.sprite = Resources.Load<Sprite>("Sprites/sk8rboiBig");
        }
    }
    public void SetPosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, 0);
        m_PlayerInfo.position = pos;
    }

    public void MovePlayer(float deltaTime)
    {
        transform.position = m_PlayerInfo.position;
        m_PlayerInfo.currentSpeed = Mathf.Lerp(m_PlayerInfo.currentSpeed, MaxSpeed, (deltaTime / m_TimeToMaxSpeed));
        m_PlayerInfo.position += new Vector2(transform.up.x, transform.up.y) * m_PlayerInfo.currentSpeed * deltaTime;
        transform.rotation = Quaternion.Euler(0.0f,0.0f, m_PlayerInfo.zRot);
        transform.position = Vector3.Lerp(transform.position, m_PlayerInfo.position, 0.6f); //in case the update is off from current position
        m_PlayerInfo.position = transform.position;
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
        if(!m_IsSpinning && !m_IsDodging)
        {
            StartCoroutine(SpinPlayerDuration(m_SpinDuration));
        }
    }

    public void updatePlayerInfo(PlayerInfo info)
    {
        m_PlayerInfo = info;
    }

    public void CheckBackDraft(float deltaTime)
    {
        bool drafting = false;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            Player oPlayer = GameManager.Instance.m_Players[i];
            if(oPlayer != this)
            {
                Bounds draftBounds = new Bounds(oPlayer.m_PlayerInfo.position - new Vector2(0.0f, m_DraftBounds.y * 0.5f), m_DraftBounds);
                if (draftBounds.Contains(m_PlayerInfo.position))
                {
                    m_PlayerInfo.currentSpeed = Mathf.Lerp(m_PlayerInfo.currentSpeed, MaxSpeed * m_BackDraftMultiplier, deltaTime);
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
