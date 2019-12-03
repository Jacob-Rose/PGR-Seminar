using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
/*
 * Note: More than one client player can exist, make sure to not hardcode in controls except for early testing
 */
public class ClientPlayer : Player
{
    [SerializeField]
    public InputMaster controls;
    public InputDevice m_LatestDevice = null;
    public float zRotAmount = 10.0f;
    public float m_SpeedDecreaseAmount = 0.1f;

    public float m_InteractMaxDist = 4.0f;
    public float m_DodgeTimeInAir = 1.5f;

    public float m_PlayerXBounds = 7.0f;


    private float m_TimeSinceDodge = 0.0f;
    private float m_TimeUntilDodge = 3.0f;
    private float m_InteractTimer = 0.0f;
    private float m_InteractLimit = 1.0f;
    private float m_CollisionMinimum = 0.3f;
    private float m_WallCollisionSpeedReduce = 0.80f;
    private float m_PlayerCollisionSpeedReduce = 0.95f;
    private float m_AttackRange = 1.5f;
    private float m_CurrentClosestDistance = 0.0f;
    private IObstacle m_ClosestObstacle = null;

    public AudioClip ollieClip;
    public AudioClip rollingClip;

    public void Awake()
    {
        controls = new InputMaster();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Interact.performed += InteractButtonPressed;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if (!GameManager.Instance.HasGameStarted)
            return;

        HandleInput(Time.deltaTime);
        m_InteractTimer += Time.deltaTime;
        FindClosestObstacle();
        PlayerCollision();
        //PlayerAttack();
        CheckPlayerSound();
        base.Update();
    }

    public void CheckPlayerSound()
    {
        AudioSource aux = GetComponent<AudioSource>();
        if(aux != null)
        {
            if(!playerInfo.collidable)
            {
                TryPlayOllieSound();
            }
            else
            {
                TryPlayRollingSound();
            }
        }
        /*
        if(aux.isPlaying)
        {
            Debug.LogError("audio is playing");
        }
        */
    }

    public void TryPlayRollingSound()
    {
        AudioSource aux = GetComponent<AudioSource>();
        if(aux.clip != rollingClip)
        {
            aux.Stop();
            aux.clip = rollingClip;
            aux.time = 0.0f;
            aux.loop = true;
            aux.Play();
        }
    }

    public void TryPlayOllieSound()
    {
        AudioSource aux = GetComponent<AudioSource>();
        if (aux.clip != ollieClip)
        {
            aux.Stop();
            aux.clip = ollieClip;
            aux.loop = false;
            aux.time = 0;
            aux.Play();
        }
    }

    public void HandleInput(float deltaTime)
    {
        if(!m_IsDodging)
        {
            m_TimeSinceDodge += deltaTime;
        }

        //attackTimer += deltaTime;
        if (controls.Player.Jump.ReadValue<float>() > 0.5f && !m_IsSpinning && !m_IsDodging)
        {
            if (m_TimeSinceDodge > m_TimeUntilDodge)
            {
                m_TimeSinceDodge = 0.0f;
                StartCoroutine(Dodge(m_DodgeTimeInAir));
            }
        }
        float turnValue = controls.Player.Turn.ReadValue<float>();
        if (Mathf.Abs(turnValue) > 0.05f)
        {
            playerInfo.zRot -= zRotAmount * deltaTime * turnValue;
            playerInfo.currentSpeed -= m_SpeedDecreaseAmount * deltaTime * turnValue;
        }
        else
        {
            playerInfo.zRot = Mathf.LerpAngle(playerInfo.zRot, 0, 0.1f);
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (m_ClosestObstacle != null && !m_IsSpinning && !m_IsDodging && m_InteractTimer >= m_InteractLimit)
        {
            //Interact with the obstacle
            m_InteractTimer = 0.0f;
            m_ClosestObstacle.GetComponent<SpriteRenderer>().color = Color.white;
            m_ClosestObstacle.HandleInteraction(this);

            ObstacleModifiedMessage msg = new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, m_ClosestObstacle.id);
            if (VHostBehavior.Instance != null)
            {
                VHostBehavior.Instance.SendMessageToAllPlayers(msg, Valve.Sockets.SendType.Reliable);
            }
            else
            {
                VOnlinePlayer.Instance.SendMessage(msg, Valve.Sockets.SendType.Reliable);
            }
        }
        m_LatestDevice = context.control.device;
    }

    public void PlayerCollision()
    {
        Player closestPlayer = null;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            if ((Vector2.Distance(playerInfo.position, GameManager.Instance.m_Players[i].transform.position) <= m_CollisionMinimum) 
                /*&& closestPlayer == null || (Vector2.Distance(playerInfo.position, GameManager.Instance.m_Players[i].transform.position) <= Vector2.Distance(playerInfo.position, closestPlayer.transform.position))*/
                && GameManager.Instance.m_Players[i] != this)
            {
                closestPlayer = GameManager.Instance.m_Players[i];
            }
        }
        if(closestPlayer != null || playerInfo.position.x <= -m_PlayerXBounds || playerInfo.position.x >= m_PlayerXBounds)
        {
            if(playerInfo.position.x <= -m_PlayerXBounds)
            {
                playerInfo.zRot = -playerInfo.zRot;
                playerInfo.position.x = -m_PlayerXBounds + 0.1f;
                playerInfo.currentSpeed *= m_WallCollisionSpeedReduce;

            }
            else if(playerInfo.position.x >= m_PlayerXBounds)
            {
                playerInfo.zRot = -playerInfo.zRot;
                playerInfo.position.x = m_PlayerXBounds - 0.1f;
                playerInfo.currentSpeed *= m_WallCollisionSpeedReduce;

            }
            else if(closestPlayer != null)
            {
                if (closestPlayer.playerInfo.collidable && playerInfo.collidable)
                {
                    if(playerInfo.position.x <= closestPlayer.playerInfo.position.x)
                    {
                        playerInfo.zRot = -playerInfo.zRot;
                        playerInfo.position.x = closestPlayer.playerInfo.position.x - 0.4f;
                        playerInfo.currentSpeed *= m_PlayerCollisionSpeedReduce;

                    }
                    else if(playerInfo.position.x >= closestPlayer.playerInfo.position.x)
                    {
                        playerInfo.zRot = -playerInfo.zRot;
                        playerInfo.position.x = closestPlayer.playerInfo.position.x + 0.4f;
                        playerInfo.currentSpeed *= m_PlayerCollisionSpeedReduce;

                    }
                }
            }

        }
    }

    public void PlayerAttack()
    {
        Player closestPlayer = null;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            if ((Vector2.Distance(playerInfo.position, GameManager.Instance.m_Players[i].transform.position) <= m_AttackRange) && GameManager.Instance.m_Players[i] != this)
            {
                closestPlayer = GameManager.Instance.m_Players[i];
            }
        }
        Debug.Log(closestPlayer);
        if(closestPlayer != null)
        {
            if (closestPlayer.playerInfo.collidable && playerInfo.collidable)
            {
                if (controls.Player.Attack.ReadValue<float>() > 0.5f && !m_IsSpinning && !m_IsDodging)
                {
                    //run punch animation
                    if(closestPlayer.playerInfo.position.x > playerInfo.position.x)
                    {
                        m_EnemyisRight = true;
                    }
                    else
                    {
                        m_EnemyisRight = false;
                    }
                    StartCoroutine(Attack(m_AttackDuration));
                    Debug.Log("attack happen)");
                    if (m_InteractTimer > m_InteractLimit)
                    {
                        m_TimeSinceDodge = 0.0f;
                        m_InteractTimer = 0.0f;
                        //Run crash animation
                        GameManager.Instance.PlayerAttackedByPlayer(this, closestPlayer);
                    }
                }
            }
        }
    }

    public void FindClosestObstacle()
    {
        m_CurrentClosestDistance = m_InteractMaxDist;
        m_ClosestObstacle = null;
        for (int i = 0; i < GameManager.Instance.getAllObstacleCount(); i++)
        {
            if(GameManager.Instance.m_AllObstacles[i] != null && GameManager.Instance.m_AllObstacles[i] is IObstacle 
                && (GameManager.Instance.m_AllObstacles[i] as IObstacle).m_CanBeInteractedWith)
            {
                float oDist = Vector2.Distance(playerInfo.position, GameManager.Instance.m_AllObstacles[i].transform.position);
                if (oDist < m_CurrentClosestDistance)
                {
                    m_ClosestObstacle = GameManager.Instance.m_AllObstacles[i] as IObstacle;
                    m_CurrentClosestDistance = oDist;
                }
                GameManager.Instance.m_AllObstacles[i].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        if(m_ClosestObstacle != null)
        {
            m_ClosestObstacle.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public IEnumerator Dodge(float duration)
    {
        m_IsDodging = true;
        float time = 0.0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            playerInfo.collidable = false;
            yield return 0;
        }
        playerInfo.collidable = true;
        m_IsDodging = false;
    }

    public IEnumerator Attack(float duration)
    {
        playerInfo.attacking = true;
        m_IsAttacking = true;
        float time = 0.0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            yield return 0;
        }
        playerInfo.attacking = false;
        m_IsAttacking = false;
    }

    private void OnGUI()
    {
        Rect maxSpeedRect = new Rect(Screen.width / 2, 0, (Screen.width / 2) - 10, 40);
        Rect forwardSpeedRect = new Rect(Screen.width / 2, 40, (Screen.width / 2) - 10, 40);
        Rect speedRect = new Rect(Screen.width / 2, 80, (Screen.width / 2)- 10, 40);
        GUI.color = Color.black;
        Rect bgBounds = new Rect((Screen.width / 5) * 4, 0, Screen.width / 5, 120);
        GUI.Box(bgBounds, "");
        bgBounds = new Rect(0, 0, Screen.width / 6, 60);
        GUI.Box(bgBounds, "");

        GUI.color = Color.white;
        GUIStyle speedStyle = GUI.skin.label;
        speedStyle.alignment = TextAnchor.MiddleRight;
        speedStyle.fontSize = 20;
        GUI.Label(maxSpeedRect, "Max-Speed: " + MaxSpeed.ToString("F1"), speedStyle);
        GUI.Label(forwardSpeedRect, "F-Speed: " + (playerInfo.currentSpeed * transform.up.y).ToString("F1"), speedStyle);
        GUI.Label(speedRect, "C-Speed: " + playerInfo.currentSpeed.ToString("F1"), speedStyle);

        GUIStyle scoreStyle = GUI.skin.label;
        scoreStyle.fontSize = 22;
        scoreStyle.alignment = TextAnchor.MiddleLeft;
        Rect scoreRect = new Rect(10, 0, (Screen.width / 5), 60);
        GUI.Label(scoreRect, "Score: " + playerInfo.currentScore.ToString(), scoreStyle);

        //Start of Dodge Bar code
        Vector2 dodgeBarSize = new Vector2(Screen.width * 0.1f, 20);
        Texture2D emptyTex = Texture2D.blackTexture;//Resources.Load<Texture2D>("Sprites/yellow");
        Texture2D fullTex = Resources.Load<Texture2D>("Sprites/blue");

        Rect dodgeBarRect = new Rect(10, Screen.height * 0.4f, dodgeBarSize.x, dodgeBarSize.y);
        GUI.BeginGroup(dodgeBarRect);
        GUI.Box(new Rect(0, 0, dodgeBarRect.width, dodgeBarRect.height), emptyTex);
        GUI.DrawTexture(new Rect(0, 0, dodgeBarSize.x * (Mathf.Clamp(m_TimeSinceDodge, 0, m_TimeUntilDodge) / m_TimeUntilDodge), dodgeBarSize.y), fullTex);
        GUI.color = Color.black;
        GUI.Label(new Rect(0, 0, dodgeBarRect.width, dodgeBarRect.height), "Dodge ", scoreStyle);
        GUI.color = Color.white;
        GUI.EndGroup();
        //end of dodge bar

         
        //Start of Interact Bar code
        Vector2 interactBarSize = new Vector2(Screen.width * 0.2f, 20);
        Texture2D emptyTex2 = Texture2D.blackTexture;
        Texture2D fullTex2 = Resources.Load<Texture2D>("Sprites/yellow");

        Rect interactBarRect = new Rect(10, Screen.height * 0.6f, interactBarSize.x, interactBarSize.y);
        GUI.BeginGroup(interactBarRect);
        GUI.Box(new Rect(0, 0, interactBarRect.width, interactBarRect.height), emptyTex2);
        GUI.DrawTexture(new Rect(0, 0, interactBarSize.x * (Mathf.Clamp(m_InteractTimer, 0, m_InteractLimit) / m_InteractLimit), interactBarSize.y), fullTex2);
        GUI.color = Color.black;
        GUI.Label(new Rect(0, 0, interactBarRect.width, interactBarRect.height), "Interact/Attack ", scoreStyle);
        GUI.color = Color.white;
        GUI.EndGroup();
        //end of interact bar


    }

    public override string GetUsername()
    {
        return GameManager.Instance.m_PlayerUsername;
    }
}
