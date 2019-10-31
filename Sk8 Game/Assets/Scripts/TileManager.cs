using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] obstList;
    //is the start of the race
    public GameObject firstTransform;
    //void that ends game when player hits
    public GameObject endTest;
    //just a sprite for the road
    public GameObject road;

    //[SerializeField]
    public int roadSize = 300;
    //[SerializeField]
    public int stageLevelAmount = 1;

    public static TileManager m_Instance;
    // Start is called before the first frame update
    void Start()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
        }
        PopulateRoads();
    }

    // Update is called once per frame
    void Update()
    {
        // v this doesnt really make sense atm, ignore
        //stageLevelAmount = obstManager.GetComponent<ObstGen>().GetStagePoint();
    }

    public struct ObstInfo
    {
        public Vector3 spawnedPos;
        public GameObject spawnedObstacle;
        //add bool interacted with
    }

    //Get obstacle data and spawnPos, instantiate at Vector based on randomized func
    ObstInfo SpawnObstaclesOnTile(Bounds tileBounds)
    {
        ObstInfo returnVal = new ObstInfo();
        Vector3 obstaclePos = CreateObstaclePoint(tileBounds);
        float obstListNum = (float)obstList.Length;
        int randomIndex = (int)Random.Range(0, obstListNum); //assume all players have the same list, used in message
        Obstacle newObs = SpawnObstacle((uint)Obstacle.getAllObstacleCount(), obstaclePos, randomIndex);
        return returnVal;
    }

    public Vector2 CreateObstaclePoint(Bounds possibleSpawnBounds)
    {
        Vector2 randomPos = new Vector2(Random.Range(-possibleSpawnBounds.extents.x, possibleSpawnBounds.extents.x), 
            Random.Range(-possibleSpawnBounds.extents.y, possibleSpawnBounds.extents.y)) ;
        randomPos += new Vector2(possibleSpawnBounds.center.x, possibleSpawnBounds.center.y);
        return randomPos;
    }

    public Obstacle SpawnObstacle(uint itemID, Vector2 pos, int itemType)
    {
        GameObject determinedObst = obstList[itemType];

        Obstacle newObstacle = Instantiate(determinedObst, pos, Quaternion.identity, transform).GetComponent<Obstacle>();
        newObstacle.id = itemID;
        return newObstacle;
    }

    //populate roads using GetObsStatsSpawn, is based on roads
    public void PopulateRoads()
    {
        Bounds roadBounds = new Bounds(firstTransform.transform.position, new Vector3(5, 5));
        for (int i = 0; i < roadSize; i++)
        {
            Instantiate(road, roadBounds.center, Quaternion.identity, transform);

            if (i == roadSize-3) //few roads to slow down on
            {
                Instantiate(endTest, roadBounds.center, Quaternion.identity, transform);
            }
            if(VHostBehavior.m_Instance != null && i != 0) //only call spawn if host, sends to other players
            {
                SpawnObstaclesOnTile(roadBounds);
            }
            
            roadBounds.center = new Vector3(roadBounds.center.x, roadBounds.center.y + 9, 0.0f);
            
        }
        //TODO Send Message with all Obstacle information
    }
}
