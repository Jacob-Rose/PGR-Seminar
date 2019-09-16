using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : Player
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //send the data to the client

    }

    void MoveLeftEvent(Vector3 pos, float speed) //not sure if values passed in are needed
    {
        playerPos -= new Vector3(0f, 2f, 0f);
        playerSpeed -= 0.75f;
    }

    void MoveRightEvent(Vector3 pos, float speed)
    {
        playerPos = pos;
        playerSpeed = speed;
        playerPos += new Vector3(0f, 2f, 0f); //update position
        playerSpeed -= 0.75f; //subtract player speed
    }

    void TrickEvent(Vector3 pos, string trickName)
    {
        //todo, string will tell the game what trick to perform
    }

    void AttackEvent(Vector3 playerPos, Vector3 player2Pos, float range)
    {
        //todo
    }
}
