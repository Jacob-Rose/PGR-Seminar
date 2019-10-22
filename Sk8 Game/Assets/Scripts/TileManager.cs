using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //is the start of the race
    public GameObject firstTransform;
    //Gen manager
    public GameObject obstManager;
    //void that ends game when player hits
    public GameObject endTest;
    //just a sprite for the road
    public GameObject road;
    //is rock
    public GameObject testObstacle;

    //[SerializeField]
    public int roadSize = 300;
    //[SerializeField]
    public int stageLevelAmount = 1;
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("GetObstStatsSpawn", 2.0f, 2.0f);
        PopulateRoads();
    }

    // Update is called once per frame
    void Update()
    {
        // v this doesnt really make sense atm, ignore
        //stageLevelAmount = obstManager.GetComponent<ObstGen>().GetStagePoint();
    }

    //Get obstacle data and spawnPos, instantiate at Vector based on randomized func
    void GetObstStatsSpawn(Vector2 spawnPosition, GameObject obstacle)
    {
        Vector3 obstaclePos = obstManager.GetComponent<ObstGen>().CreateObstaclePoint(spawnPosition);
        Instantiate(obstacle, obstaclePos, Quaternion.identity);
    }

    //populate roads using GetObsStatsSpawn, is based on roads
    public void PopulateRoads()
    {
        Vector2 spawnPos = firstTransform.transform.position;
        for (int i = 0; i < roadSize; i++)
        {
            Instantiate(road, spawnPos, Quaternion.identity);
            //GetObstStatsSpawn(spawnPos, testObstacle);

            if (i == roadSize)
            {
                //Vector3 obstaclePos = obstManager.GetComponent<ObstGen>().CreateObstaclePoint();
                Instantiate(endTest, spawnPos, Quaternion.identity);

            }
            //this needs to change to occur based on distance from start point
            for (int j = 0; j < stageLevelAmount; j++)
            {
                GetObstStatsSpawn(spawnPos, testObstacle);
            }
            spawnPos.y += 9;
            
        }
    }
}
