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
        if (controls.Player.LEFT.ReadValue<float>() > 0.1f)
        {
            playerInfo.zRot += zRotAmount * deltaTime;
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime;
        }
        else if (controls.Player.RIGHT.ReadValue<float>() > 0.1f)
        {
            playerInfo.zRot -= zRotAmount * deltaTime;
            playerInfo.currentSpeed -= speedDecreaseAmount * deltaTime;
        }
        else
        {
            playerInfo.zRot = Mathf.LerpAngle(playerInfo.zRot, 0, 0.1f);
        }
    }
    
}
