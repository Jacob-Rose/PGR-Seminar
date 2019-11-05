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

    public float m_InteractMaxDist = 4.0f;

    private float m_DodgeTimer = 0.0f;
    private float m_AttackTimer = 0.0f;
    private float m_CurrentClosestDistance = 0.0f;
    private Obstacle m_ClosestObstacle = null;

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
        GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.m_PlayerUsername;
        if (!GameManager.Instance.HasGameStarted)
            return;

        HandleInput(Time.deltaTime);
        FindClosestObstacle();
        
        base.Update();
    }

    public void HandleInput(float deltaTime)
    {
        m_DodgeTimer += deltaTime;
        //attackTimer += deltaTime;
        if (controls.Player.Jump.ReadValue<float>() > 0.5f && !m_IsSpinning && !m_IsDodging)
        {
            if (m_DodgeTimer > 3.0f)
            {
                m_DodgeTimer = 0.0f;
                StartCoroutine(Dodge(1.0f));
            }
        }
        float turnValue = controls.Player.Turn.ReadValue<float>();
        if (Mathf.Abs(turnValue) > 0.05f)
        {
            playerInfo.zRot -= zRotAmount * deltaTime * turnValue;
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime * turnValue;
        }
        else
        {
            playerInfo.zRot = Mathf.LerpAngle(playerInfo.zRot, 0, 0.1f);
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (m_ClosestObstacle != null && !m_ClosestObstacle.m_InteractedWith && !m_IsSpinning && !m_IsDodging)
        {
            if (controls.Player.Interact.ReadValue<float>() > 0.5f)
            {
                //Interact with the obstacle
                m_ClosestObstacle.HandleInteraction(this);
                ObstacleModifiedMessage msg = new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, m_ClosestObstacle.id);
                if (VHostBehavior.Instance != null)
                {
                    VHostBehavior.Instance.SendMessageToAllPlayers(msg);
                }
                else
                {
                    VOnlinePlayer.Instance.SendMessage(msg);
                }
                Debug.Log("Interacted with highlighted obstacle");
            }
        }
    }

    public void PlayerAttack()
    {
        Player closestPlayer = null;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            /*if (Vector2.Distance(playerInfo.position, GameManager.Instance.m_Players[i].transform.position) < )
            {
                closestPlayer = GameManager.Instance.m_Players[i];
            }*/
        }
        if(closestPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (m_AttackTimer > 3.0f)
                {
                    m_DodgeTimer = 0.0f;
                    //Run crash animation
                    playerInfo.currentSpeed *= 0.85f;

                }
            }
        }
    }

    public void FindClosestObstacle()
    {
        m_CurrentClosestDistance = m_InteractMaxDist;
        m_ClosestObstacle = null;
        for (int i = 0; i < Obstacle.getAllObstacleCount(); i++)
        {
            if(Obstacle.m_AllObstacles[i] != null)
            {
                float oDist = Vector2.Distance(playerInfo.position, Obstacle.m_AllObstacles[i].transform.position);
                if (oDist < m_CurrentClosestDistance)
                {
                    Obstacle.m_AllObstacles[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                    m_ClosestObstacle = Obstacle.m_AllObstacles[i];
                    m_CurrentClosestDistance = oDist;
                }
                else
                {
                    Obstacle.m_AllObstacles[i].GetComponent<SpriteRenderer>().color = Color.white;

                }
            }
        }
    }

    public IEnumerator Dodge(float duration)
    {
        m_IsDodging = true;
        float time = 0.0f;
        GameObject spriteChild = this.transform.GetChild(0).gameObject;
        while (time <= duration)
        {
            time += Time.deltaTime;
            playerInfo.collidable = false;
            spriteChild.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            yield return 0;
        }
        spriteChild.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        playerInfo.collidable = true;
        m_IsDodging = false;
    }

    private void OnGUI()
    {
        GUIStyle scoreStyle = GUI.skin.label;
        scoreStyle.fontSize = 22;
        scoreStyle.alignment = TextAnchor.MiddleLeft;
        Rect scoreRect = new Rect(10, 0, (Screen.width /2) - 10, 100);
        GUI.Label(scoreRect, "Score: " + playerInfo.currentScore.ToString(), scoreStyle);
        GUIStyle speedStyle = GUI.skin.label;
        speedStyle.alignment = TextAnchor.MiddleRight;
        speedStyle.fontSize = 20;
        Rect maxSpeedRect = new Rect(Screen.width / 2, 0, (Screen.width / 2) - 10, 40);
        Rect forwardSpeedRect = new Rect(Screen.width / 2, 40, (Screen.width / 2) - 10, 40);
        Rect speedRect = new Rect(Screen.width / 2, 80, (Screen.width / 2)- 10, 40);
        GUI.Label(maxSpeedRect, "Max-Speed: " + MaxSpeed.ToString("F1"), speedStyle);
        GUI.Label(forwardSpeedRect, "F-Speed: " + (playerInfo.currentSpeed * transform.up.y).ToString("F1"), speedStyle);
        GUI.Label(speedRect, "C-Speed: " + playerInfo.currentSpeed.ToString("F1"), speedStyle);
    }

}
