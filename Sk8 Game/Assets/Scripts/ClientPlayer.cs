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

    public float zRotAmount = 10.0f;
    public float m_SpeedDecreaseAmount = 0.1f;

    public float m_InteractMaxDist = 4.0f;

    public float m_PlayerXBounds = 7.0f;

    public float m_AttackStaminaCost = 1.0f;
    public float m_DodgeStaminaCost = 1.0f;
    public float m_InteractStaminaCost = 1.0f;


    private float m_TimeSinceDodge = 0.0f;
    private float m_CollisionMinimum = 0.3f;
    private float m_WallCollisionSpeedReduce = 0.80f;
    private float m_PlayerCollisionSpeedReduce = 0.95f;
    private float m_AttackRange = 1.5f;
    private float m_CurrentClosestDistance = 0.0f;
    private IObstacle m_ClosestObstacle = null;


    public void Awake()
    {
        controls = new InputMaster();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Interact.performed += InteractButtonPressed;
        controls.Player.Attack.performed += AttackButtonPressed;
        controls.Player.Jump.performed += DodgeButtonPressed;
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
        float deltaTime = Time.deltaTime;

        if (!GameManager.Instance.HasGameStarted)
            return;

        if (m_PlayerInfo.stamina < m_MaxStamina && !m_IsDodging && !m_IsSpinning)
        {
            m_PlayerInfo.stamina = Mathf.Clamp(m_PlayerInfo.stamina + m_StaminaRefillPerSecond * deltaTime, 0, m_MaxStamina);
        }

        HandleInput(deltaTime);
        FindClosestObstacle();
        PlayerCollision();
        base.Update();
    }

    public void HandleInput(float deltaTime)
    {
        if(!m_IsDodging)
        {
            m_TimeSinceDodge += deltaTime;
        }

        //attackTimer += deltaTime;
        
        float turnValue = controls.Player.Turn.ReadValue<float>();
        if (Mathf.Abs(turnValue) > 0.05f)
        {
            m_PlayerInfo.zRot -= zRotAmount * deltaTime * turnValue;
            m_PlayerInfo.currentSpeed -= m_SpeedDecreaseAmount * deltaTime * turnValue;
        }
        else
        {
            m_PlayerInfo.zRot = Mathf.LerpAngle(m_PlayerInfo.zRot, 0, 0.1f);
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (m_ClosestObstacle != null 
            && !m_IsSpinning 
            && !m_IsDodging 
            && m_InteractStaminaCost <= m_PlayerInfo.stamina)
        {
            //Interact with the obstacle
            m_PlayerInfo.stamina -= m_InteractStaminaCost;
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
    }

    public void DodgeButtonPressed(InputAction.CallbackContext context)
    {
        if (!m_IsSpinning && !m_IsDodging && m_DodgeStaminaCost <= m_PlayerInfo.stamina)
        {
            m_PlayerInfo.stamina -= m_DodgeStaminaCost;
            StartCoroutine(Dodge(m_DodgeTimeInAir));
        }
    }

    public void AttackButtonPressed(InputAction.CallbackContext context)
    {
        Player closestPlayer = null;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            if ((Vector2.Distance(m_PlayerInfo.position, GameManager.Instance.m_Players[i].transform.position) <= m_AttackRange) && GameManager.Instance.m_Players[i] != this)
            {
                closestPlayer = GameManager.Instance.m_Players[i];
            }
        }
        Debug.Log(closestPlayer);
        if (closestPlayer != null)
        {
            if (closestPlayer.m_PlayerInfo.collidable && m_PlayerInfo.collidable)
            {
                StartCoroutine(Attack(m_AttackDuration));
                Debug.Log("attack happen)");
                if (m_AttackStaminaCost <= m_PlayerInfo.stamina)
                {
                    m_PlayerInfo.stamina -= m_AttackStaminaCost;
                    PlayerAttackedPlayerMessage msg = new PlayerAttackedPlayerMessage(GetUsername(), closestPlayer.GetUsername());
                    if (VOnlinePlayer.Instance == null)
                    {
                        VHostBehavior.Instance.SendMessageToAllPlayers(msg, Valve.Sockets.SendType.Reliable);
                    }
                    else
                    {
                        VOnlinePlayer.Instance.SendMessage(msg, Valve.Sockets.SendType.Reliable);
                    }
                }
            }
        }
    }
    public void PlayerCollision()
    {
        Player closestPlayer = null;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            if ((Vector2.Distance(m_PlayerInfo.position, GameManager.Instance.m_Players[i].transform.position) <= m_CollisionMinimum) 
                /*&& closestPlayer == null || (Vector2.Distance(playerInfo.position, GameManager.Instance.m_Players[i].transform.position) <= Vector2.Distance(playerInfo.position, closestPlayer.transform.position))*/
                && GameManager.Instance.m_Players[i] != this)
            {
                closestPlayer = GameManager.Instance.m_Players[i];
            }
        }
        if(closestPlayer != null || m_PlayerInfo.position.x <= -m_PlayerXBounds || m_PlayerInfo.position.x >= m_PlayerXBounds)
        {
            if(m_PlayerInfo.position.x <= -m_PlayerXBounds)
            {
                m_PlayerInfo.zRot = -m_PlayerInfo.zRot;
                m_PlayerInfo.position.x = -m_PlayerXBounds + 0.1f;
                m_PlayerInfo.currentSpeed *= m_WallCollisionSpeedReduce;

            }
            else if(m_PlayerInfo.position.x >= m_PlayerXBounds)
            {
                m_PlayerInfo.zRot = -m_PlayerInfo.zRot;
                m_PlayerInfo.position.x = m_PlayerXBounds - 0.1f;
                m_PlayerInfo.currentSpeed *= m_WallCollisionSpeedReduce;

            }
            else if(closestPlayer != null)
            {
                if (closestPlayer.m_PlayerInfo.collidable && m_PlayerInfo.collidable)
                {
                    if(m_PlayerInfo.position.x <= closestPlayer.m_PlayerInfo.position.x)
                    {
                        m_PlayerInfo.zRot = -m_PlayerInfo.zRot;
                        m_PlayerInfo.position.x = closestPlayer.m_PlayerInfo.position.x - 0.4f;
                        m_PlayerInfo.currentSpeed *= m_PlayerCollisionSpeedReduce;

                    }
                    else if(m_PlayerInfo.position.x >= closestPlayer.m_PlayerInfo.position.x)
                    {
                        m_PlayerInfo.zRot = -m_PlayerInfo.zRot;
                        m_PlayerInfo.position.x = closestPlayer.m_PlayerInfo.position.x + 0.4f;
                        m_PlayerInfo.currentSpeed *= m_PlayerCollisionSpeedReduce;

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
                float oDist = Vector2.Distance(m_PlayerInfo.position, GameManager.Instance.m_AllObstacles[i].transform.position);
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
            m_PlayerInfo.collidable = false;
            yield return 0;
        }
        m_PlayerInfo.collidable = true;
        m_IsDodging = false;
    }

    public IEnumerator Attack(float duration)
    {
        m_PlayerInfo.attacking = true;
        m_IsAttacking = true;
        float time = 0.0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            yield return 0;
        }
        m_PlayerInfo.attacking = false;
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
        GUI.Label(forwardSpeedRect, "F-Speed: " + (m_PlayerInfo.currentSpeed * transform.up.y).ToString("F1"), speedStyle);
        GUI.Label(speedRect, "C-Speed: " + m_PlayerInfo.currentSpeed.ToString("F1"), speedStyle);

        GUIStyle scoreStyle = GUI.skin.label;
        scoreStyle.fontSize = 22;
        scoreStyle.alignment = TextAnchor.MiddleLeft;
        Rect scoreRect = new Rect(10, 0, (Screen.width / 5), 60);
        GUI.Label(scoreRect, "Score: " + m_PlayerInfo.currentScore.ToString(), scoreStyle);

        //Start of stam bar code
        Vector2 dodgeBarSize = new Vector2(Screen.width * 0.3f, 20);
        Texture2D emptyTex = Texture2D.blackTexture;//Resources.Load<Texture2D>("Sprites/yellow");
        Texture2D fullTex = Resources.Load<Texture2D>("Sprites/blue");

        Rect boxSegment = new Rect(Screen.width * 0.2f, Screen.height * 0.9f, dodgeBarSize.x/4, dodgeBarSize.y);

        Rect dodgeBarRect = new Rect(Screen.width * 0.35f, Screen.height * 0.9f, dodgeBarSize.x, dodgeBarSize.y);
        GUI.BeginGroup(dodgeBarRect);
        GUI.Box(new Rect(0, 0, dodgeBarRect.width, dodgeBarRect.height), emptyTex);

        GUIStyle stamStyle = GUI.skin.label;
        stamStyle.fontSize = 14;
        stamStyle.alignment = TextAnchor.MiddleCenter;
        GUI.DrawTexture(new Rect(0, 0, dodgeBarSize.x * (Mathf.Clamp(m_PlayerInfo.stamina, 0, m_MaxStamina) / m_MaxStamina), dodgeBarSize.y), fullTex);
        GUI.color = Color.black;
        GUI.Box(new Rect(boxSegment.width, boxSegment.height * 0.25f, boxSegment.width / 20, boxSegment.height * 0.5f), emptyTex);
        GUI.Box(new Rect(boxSegment.width * 2, boxSegment.height * 0.25f, boxSegment.width / 20, boxSegment.height * 0.5f), emptyTex);
        GUI.Box(new Rect(boxSegment.width * 3, boxSegment.height * 0.25f, boxSegment.width / 20, boxSegment.height * 0.5f), emptyTex);
        GUI.Box(new Rect(boxSegment.width * 4, boxSegment.height * 0.25f, boxSegment.width / 20, boxSegment.height * 0.5f), emptyTex);
        GUI.color = Color.white;
        GUI.Label(new Rect(0, 0, dodgeBarRect.width, dodgeBarRect.height), "Stamina ", stamStyle);
        GUI.EndGroup();

        //end of dodge bar
    }

    public override string GetUsername()
    {
        return GameManager.Instance.m_PlayerUsername;
    }
}
