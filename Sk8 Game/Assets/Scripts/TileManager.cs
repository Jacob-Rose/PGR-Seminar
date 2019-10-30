using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] obstList;
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

    public int IDInt = 0;
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

    public enum ObstacleType
    { 
        CONE,
        ROCK
    }

    public struct ObstInfo
    {
        public int obstID;
        public Vector3 spawnedPos;
        public ObstacleType obstacleType;
        //add bool interacted with
    }

    //Get obstacle data and spawnPos, instantiate at Vector based on randomized func
    ObstInfo GetObstStatsSpawn(Vector2 spawnPosition, GameObject[] obstacleList)
    {
        ObstInfo returnVal = new ObstInfo();
        Vector3 obstaclePos = obstManager.GetComponent<ObstGen>().CreateObstaclePoint(spawnPosition);
        float obstListNum = (float)obstacleList.Length;

        ObstacleType obstype = (ObstacleType)Random.Range(0,1); // second value will be the max number of seperate different game objects

        //GameObject determinedObst = obstList[(int)Random.Range(0,obstListNum)];

        //Instantiate(determinedObst, obstaclePos, Quaternion.identity, transform);
        //Debug.Log(obstaclePos, determinedObst);
        returnVal.obstacleType = obstype;
        returnVal.spawnedPos = obstaclePos;
        returnVal.obstID = IDInt;
        IDInt++;
        return returnVal;
    }

    //populate roads using GetObsStatsSpawn, is based on roads
    public void PopulateRoads()
    {
        Vector2 spawnPos = firstTransform.transform.position;
        for (int i = 0; i < roadSize; i++)
        {
            Instantiate(road, spawnPos, Quaternion.identity, transform);
            //GetObstStatsSpawn(spawnPos, testObstacle);

            if (i == roadSize)
            {
                //Vector3 obstaclePos = obstManager.GetComponent<ObstGen>().CreateObstaclePoint();
                Instantiate(endTest, spawnPos, Quaternion.identity, transform);

            }
            //this needs to change to occur based on distance from start point
            for (int j = 0; j < stageLevelAmount; j++)
            {
                GetObstStatsSpawn(spawnPos, obstList);
            }
            spawnPos.y += 9;
            
        }
    }
}
