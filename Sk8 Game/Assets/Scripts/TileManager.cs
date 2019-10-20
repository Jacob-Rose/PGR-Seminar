using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject firstTransform;
    public GameObject obstManager;
    public GameObject endTest;
    public GameObject road;
    public GameObject testObstacle;
    public int roadSize = 100;
    public int stageLevelAmount = 3;
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("GetObstStatsSpawn", 2.0f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        PopulateRoads();
    }
    void GetObstStatsSpawn(Vector2 spawnPosition, GameObject obstacle)
    {
        Vector3 obstaclePos = obstManager.GetComponent<ObstGen>().CreateObstaclePoint(spawnPosition);
        Instantiate(obstacle, obstaclePos, Quaternion.identity);
    }

    public void PopulateRoads()
    {
        Vector2 spawnPos = firstTransform.transform.position;
        for (int i = 0; i < roadSize; i++)
        {
            Instantiate(road, spawnPos, Quaternion.identity);
            GetObstStatsSpawn(spawnPos, testObstacle);

            if (i == roadSize)
            {
                //Vector3 obstaclePos = obstManager.GetComponent<ObstGen>().CreateObstaclePoint();
                Instantiate(endTest, spawnPos, Quaternion.identity);

            }
            /*for (int j = 0; j < stageLevelAmount; j++)
            {
                GetObstStatsSpawn(spawnPos, testObstacle);
            }*/
            spawnPos.y += 10;
            
        }
    }
}
