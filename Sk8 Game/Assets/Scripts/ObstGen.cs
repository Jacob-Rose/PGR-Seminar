using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstGen : MonoBehaviour
{
    public Obstacle[] obstacles;
    public Transform viewerTransform;
    public int stage = 0;
 

    /*
     * Get position of player in the lead,spawn from this position + 10 to the y value
     * generate the obstacle in a random range, with bounds being 4 away in both directions from the spawn position
     * save that position and send to the server
     * 
     */

    // Start is called before the first frame update
    void Start()
    {

    }

    /*public Vector2 CreateEndPoint()
    {

    }*/

    public Vector2 CreateObstaclePoint(Vector2 viewrTrans)
    {
        Vector2 spawnPosition = new Vector2(0, viewrTrans.y + 10);
        Vector2 randomVal = new Vector2(Random.Range(-4.0f, 4.0f), Random.Range(-4.0f, 4.0f));
        spawnPosition += randomVal;
        return spawnPosition;
    }

    public void decideObstacle()
    {
        if(stage == 1)
        {
            //obstacle Rock has x chance
            //obstacle cone has y chance
        }
        else if(stage == 2)
        {
            //obstacle Rock has x chance
            //obstacle cone has y chance
        }
        else if(stage == 3)
        {
            //obstacle Rock has x chance
            //obstacle cone has y chance
        }
    }

    // Update is called once per frame
    void Update()
    {
        //viewerTransform will be set to the lead player
        //increment stage based on checkpoints
        CreateObstaclePoint(viewerTransform.position);
    }
}
