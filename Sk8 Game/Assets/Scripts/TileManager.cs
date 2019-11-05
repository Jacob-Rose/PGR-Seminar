﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] m_ObstacleList;
    //is the start of the race
    public Transform m_StartTransform;
    //void that ends game when player hits
    public GameObject endTest;
    //just a sprite for the road
    public GameObject road;

    //[SerializeField]
    public int roadSize = 300;
    //[SerializeField]
    public int obstaclesPerTile = 2;

    public static TileManager Instance { get { return m_Instance; } }

    private static TileManager m_Instance;
    // Start is called before the first frame update
    void Start()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
        }
        PopulateRoads();
    }

    public struct ObstInfo
    {
        public Vector3 spawnedPos;
        public GameObject spawnedObstacle;
    }
    int obsCount = 0;
    ObstInfo SpawnObstaclesOnTile(Bounds tileBounds)
    {

        ObstInfo returnVal = new ObstInfo();
        Vector3 obstaclePos = CreateObstaclePoint(tileBounds);
        float obstListNum = (float)m_ObstacleList.Length;
        int randomIndex = (int)Random.Range(0, obstListNum); //assume all players have the same list, used in message
        Obstacle newObs = SpawnObstacle((uint)Obstacle.getAllObstacleCount(), obstaclePos, randomIndex);
        VHostBehavior.Instance.SendMessageToAllPlayers(new ObstacleGeneratedMessage((uint)obsCount, obstaclePos, (ushort)randomIndex), Valve.Sockets.SendType.Reliable);
        obsCount++;
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
        GameObject determinedObst = m_ObstacleList[itemType];
        Obstacle newObstacle = Instantiate(determinedObst, pos, Quaternion.identity, transform).GetComponent<Obstacle>();
        newObstacle.id = itemID;
        
        return newObstacle;
    }

    const float roadHeight = 9.0f;
    //populate roads using GetObsStatsSpawn, is based on roads
    public void PopulateRoads()
    {
        Bounds roadBounds = new Bounds(m_StartTransform.position, new Vector3(5, 5));
        for (int i = 0; i < roadSize; i++)
        {
            GameObject newRoad = Instantiate(road, roadBounds.center, Quaternion.identity, transform);

            if (i == roadSize-3) //few roads to slow down on
            {
                Instantiate(endTest, roadBounds.center, Quaternion.identity, transform);
            }
            if(VHostBehavior.Instance != null && i != 0) //only call spawn if host, sends to other players
            {
                for(int j = 0; j < obstaclesPerTile; j++)
                {
                    SpawnObstaclesOnTile(roadBounds);
                }
            } 
            roadBounds.center = new Vector3(roadBounds.center.x, roadBounds.center.y + roadHeight, 0.0f);
        }
    }
}
