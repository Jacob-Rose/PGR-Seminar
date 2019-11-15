using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] m_ObstacleList;
    //is the start of the race
    public Transform m_StartTransform;
    //void that ends game when player hits
    public GameObject m_EndPrefab;
    //just a sprite for the road
    public GameObject m_RoadPrefab;
    public GameObject m_GrassPrefab;
    public Gradient m_WallGradient;
    public GameObject m_WallPrefab;

    public int roadCount = 300;
    public Vector2 desiredRoadTileSize = new Vector2(8.0f, 12.0f);
    public float m_GrassWidth = 4.0f;
    public float m_WallWidth = 2.0f;
    //private Vector2 roadTileSize;
    public int obstaclesPerTile = 2;

    public AnimationCurve roadWidthOverTime;

    public static TileManager Instance { get { return m_Instance; } }

    private static TileManager m_Instance;
    // Start is called before the first frame update
    void Start()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
        }
        m_RoadPrefab.transform.localScale = Vector2.one;
        m_GrassPrefab.transform.localScale = Vector2.one;
        Bounds spriteBounds = m_RoadPrefab.GetComponent<SpriteRenderer>().sprite.bounds;
        m_RoadPrefab.transform.localScale = new Vector3(desiredRoadTileSize.x / spriteBounds.size.x, desiredRoadTileSize.y / spriteBounds.size.y, 1);
        spriteBounds = m_GrassPrefab.GetComponent<SpriteRenderer>().sprite.bounds;
        m_GrassPrefab.transform.localScale = new Vector3(m_GrassWidth / spriteBounds.size.x, desiredRoadTileSize.y / spriteBounds.size.y, 1) ;
        spriteBounds = m_RoadPrefab.GetComponent<SpriteRenderer>().sprite.bounds;
        m_EndPrefab.transform.localScale = new Vector3(desiredRoadTileSize.x / spriteBounds.size.x, 6.0f / spriteBounds.size.y, 1);
        spriteBounds = m_WallPrefab.GetComponent<SpriteRenderer>().sprite.bounds;
        m_WallPrefab.transform.localScale = new Vector3(m_WallWidth/ spriteBounds.size.x, desiredRoadTileSize.y / spriteBounds.size.y, 1);
        PopulateRoads(); //to replace
        
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
        Obstacle newObs = SpawnObstacle((uint)GameManager.Instance.getAllObstacleCount(), obstaclePos, randomIndex);
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
        GameManager.Instance.m_AllObstacles.Insert((int)itemID, newObstacle);
        return newObstacle;
    }

    const float roadHeight = 9.0f;
    //populate roads using GetObsStatsSpawn, is based on roads
    public void PopulateRoads()
    {
        //todo replace with not all at once spawn
        for (int i = 0; i < roadCount; i++)
        {
            SpawnRoad();
        }
    }

    public int m_CurrentRoadCount = 0;
    public void SpawnRoad()
    {
        Bounds spawnBounds = new Bounds(new Vector3(0,(m_CurrentRoadCount -1) * desiredRoadTileSize.y,0) + m_StartTransform.position, new Vector3(desiredRoadTileSize.x /*roadWidthOverTime.Evaluate(((float)m_CurrentRoadCount) / roadCount)*/, desiredRoadTileSize.y, 0));
        //Spawn Road Item

        GameObject newRoad = Instantiate(m_RoadPrefab, spawnBounds.center, Quaternion.identity, transform);
        GameObject grassLeft = Instantiate(m_GrassPrefab, spawnBounds.center + new Vector3((spawnBounds.size.x + m_GrassWidth)* 0.5f , 0,0), Quaternion.identity, transform);
        GameObject grassRight = Instantiate(m_GrassPrefab, spawnBounds.center - new Vector3((spawnBounds.size.x + m_GrassWidth) * 0.5f, 0, 0), Quaternion.identity, transform);
        GameObject wallLeft = Instantiate(m_WallPrefab, spawnBounds.center - new Vector3((spawnBounds.size.x + (m_GrassWidth * 2) + m_WallWidth) * 0.5f, 0, 0), Quaternion.identity, transform);
        wallLeft.GetComponent<SpriteRenderer>().color = m_WallGradient.Evaluate(((float)m_CurrentRoadCount) / roadCount);
        GameObject wallRight = Instantiate(m_WallPrefab, spawnBounds.center + new Vector3((spawnBounds.size.x + (m_GrassWidth * 2) + m_WallWidth) * 0.5f, 0, 0), Quaternion.identity, transform);
        wallRight.GetComponent<SpriteRenderer>().color = m_WallGradient.Evaluate(((float)m_CurrentRoadCount) / roadCount);
        if (m_CurrentRoadCount >= roadCount - 3) //spawn end for the last three roads
        {
            Instantiate(m_EndPrefab, spawnBounds.center, Quaternion.identity, transform);
        }
        //Only host spawns obstacle, and dont spawn on first tile
        if (VHostBehavior.Instance != null && m_CurrentRoadCount > 1)
        {
            for (int j = 0; j < obstaclesPerTile; j++)
            {
                SpawnObstaclesOnTile(spawnBounds);
            }
        }
        m_CurrentRoadCount++;
    }
}
