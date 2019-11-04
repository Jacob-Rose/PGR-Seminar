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

    public float interactMaxDist = 4.0f;

    private float dodgeTimer = 0.0f;
    private float attackTimer = 0.0f;

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
        LookForObstacles();
        base.Update();
    }

    public void HandleInput(float deltaTime)
    {
        dodgeTimer += deltaTime;
        attackTimer += deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dodgeTimer > 5.0f)
            {
                dodgeTimer = 0.0f;
                StartCoroutine(Dodge(1.0f));
            }
        }
        if (Mathf.Abs(controls.Player.TURN.ReadValue<float>()) > 0.1f)
        {
            playerInfo.zRot -= zRotAmount * deltaTime * controls.Player.TURN.ReadValue<float>();
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime * controls.Player.TURN.ReadValue<float>();
        }
        else
        {
            playerInfo.zRot = Mathf.LerpAngle(playerInfo.zRot, 0, 0.1f);
        }
    }

    public bool CheckIfInInteractionRange(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b) < interactMaxDist;
    }

    public void PlayerAttack()
    {
        Player closestPlayer = null;
        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            if (CheckIfInInteractionRange(playerInfo.position, GameManager.Instance.m_Players[i].transform.position))
            {
                closestPlayer = GameManager.Instance.m_Players[i];
            }
        }
        if(closestPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (attackTimer > 3.0f)
                {
                    dodgeTimer = 0.0f;
                    //Run crash animation
                    playerInfo.currentSpeed *= 0.85f;

                }
            }
        }
    }

    public void LookForObstacles()
    {
        for (int i = 0; i < Obstacle.getAllObstacleCount(); i++)
        {
            if (CheckIfInInteractionRange(playerInfo.position, Obstacle.m_AllObstacles[i].transform.position))
            {
                Obstacle.m_AllObstacles[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                if (Input.GetKey(KeyCode.E))
                {
                    //Interact with the obstacle
                    Obstacle.m_AllObstacles[i].HandleInteraction(this);
                    Debug.Log("Interacted with highlighted obstacle");
                }
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
