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
        base.Update();
    }

    public void HandleInput(float deltaTime)
    {
        
        if(Mathf.Abs(controls.Player.TURN.ReadValue<float>()) > 0.1f)
        {
            playerInfo.zRot -= zRotAmount * deltaTime * controls.Player.TURN.ReadValue<float>();
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime * controls.Player.TURN.ReadValue<float>();
        }
        else
        {
            playerInfo.zRot = Mathf.LerpAngle(playerInfo.zRot, 0, 0.1f);
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
