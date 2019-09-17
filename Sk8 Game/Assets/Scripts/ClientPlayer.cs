using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note: More than one client player can exist, make sure to not hardcode in controls except for early testing
 */
public class ClientPlayer : Player
{
    private bool isRotating; //Testing only
    private Rigidbody2D m_Rigidbody; //testing only

    [SerializeField]
    private float m_Speed; //THIS IS FOR TESTING ONLY, WILL BE REPLACED LATER 
    //set how much a rotation changes the player
    [SerializeField]
    private float zRotAmount;

    [SerializeField]
    private float speedDecreaseAmount;

    PlayerInfo clientPosInfo;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Speed = 5.0f;
        isRotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        //get keypress, send to server
        //update posInfo based on input

        //instead of calculating how long key is held, it just adds a constant amount each time. This can change, just still working on timers.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            clientPosInfo.zRot -= zRotAmount;
            clientPosInfo.currentSpeed -= speedDecreaseAmount;
            transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * m_Speed * 3.0f, Space.World); //HARD CODED FOR TEST
            isRotating = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            clientPosInfo.zRot += zRotAmount;
            clientPosInfo.currentSpeed -= speedDecreaseAmount;
            transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * m_Speed * 3.0f, Space.World); //HARD CODED FOR TEST
            isRotating = true;
        }
        if (isRotating == false) //If the player isn't hitting an arrow key, reset them to have no rotation, but keep the postion correct
        {
            transform.rotation = Quaternion.identity;
            clientPosInfo.zRot = 0.0f;
        }
        m_Rigidbody.velocity = transform.up * m_Speed; //HARD CODED FOR TEST
        isRotating = false;

        /* Commenting out for the test, will fix later, ideally in a day or two
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            clientPosInfo.zRot = 0.0f;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            clientPosInfo.zRot = 0.0f;
        }
        transform.Rotate(0, 0, clientPosInfo.zRot);
        if (clientPosInfo.zRot >= 0.0f && clientPosInfo.zRot <= 2.0f)
        {
            transform.position += new Vector3(0.0f, 0.2f, 0.0f);
        }
        else
        {
            transform.position += new Vector3(0.0f, 0.2f, 0.0f) * clientPosInfo.zRot;
        }
        */
        //send new info to the server after player update function is called
        //base.Update();


    }
    
}
