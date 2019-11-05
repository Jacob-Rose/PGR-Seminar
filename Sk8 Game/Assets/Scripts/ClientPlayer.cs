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
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public override void Update()
    {
        HandleInput(Time.deltaTime);
        FindClosestObstacle();
        
        base.Update();
    }

    public void HandleInput(float deltaTime)
    {
        m_DodgeTimer += deltaTime;
        //attackTimer += deltaTime;
        if (controls.Player.Jump.ReadValue<float>() > 0.5f)
        {
            if (m_DodgeTimer > 3.0f)
            {
                m_DodgeTimer = 0.0f;
                StartCoroutine(Dodge(1.0f));
            }
        }

        if (m_ClosestObstacle != null)
        {
            if (controls.Player.Interact.ReadValue<float>() > 0.5f)
            {
                //Interact with the obstacle
                m_ClosestObstacle.HandleInteraction(this);
                Debug.Log("Interacted with highlighted obstacle");
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
        Rect forwardSpeedRect = new Rect(Screen.width / 2, 0, (Screen.width / 2) - 10, 80);
        Rect speedRect = new Rect(Screen.width / 2, 80, (Screen.width / 2)- 10, 80);
        GUI.Label(forwardSpeedRect, "F-Speed: " + (playerInfo.currentSpeed * transform.up.y).ToString("F1"), speedStyle);
        GUI.Label(speedRect, "C-Speed: " + playerInfo.currentSpeed.ToString("F1"), speedStyle);
    }

}
