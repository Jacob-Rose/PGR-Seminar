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
        GUI.Label(new Rect(0, 0, Screen.width, 80), playerInfo.currentScore.ToString());
    }

}
