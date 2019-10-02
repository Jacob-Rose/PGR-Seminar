using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGeneration : MonoBehaviour
{
   /* public TileGeneration roadGenerator;
    public Obstacle[] obstacles;

    public float minimumObstacleDistance;

    public int maxObstacleCount;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        generateObstacles();
    }

    private void findObstacle(Vector2 chunkCoord)
    {
        ChunkType chunkType;
        if (chunkCoord.x == 0)
        {
            chunkType = ChunkType.ROAD;
        }
        else if (chunkCoord.x == -1 || chunkCoord.x == 1)
        {
            chunkType = ChunkType.SHOLDER;
        }
        else
        {
            chunkType = ChunkType.BOUNDS;
        }
        List<Obstacle> validObstacles = new List<Obstacle>();

        foreach (Obstacle o in obstacles)
        {
            if (UnityEditor.ArrayUtility.Contains(o.chunkTypes, chunkType) && UnityEditor.ArrayUtility.Contains(o.stages, TileGeneration.stage))
            {
                validObstacles.Add(o);
            }
        }

        int totalObstacleChance = 0;
        foreach (Obstacle o in validObstacles)
        {
            totalObstacleChance += o.spawnChance;
        }
        int obstacleInt = Mathf.RoundToInt(Random.Range(0, totalObstacleChance));
        totalObstacleChance = 0; //repurpose for count
        int i = 0;
        do
        {
            totalObstacleChance += validObstacles[i].spawnChance;
            if (obstacleInt < totalObstacleChance)
            {
                createObstacle(validObstacles[i], chunkCoord);
                break;
            }
            i++;
        }
        while (true);
    }

    void createObstacle(Obstacle obstacle, Vector2 chunkCoord)
    {
        float chunkOffsetX = Random.Range(-roadGenerator.chunkSize * obstacle.xSpawnRange.x / 2, roadGenerator.chunkSize * obstacle.xSpawnRange.y / 2);
        float chunkOffsetY = Random.Range(-roadGenerator.chunkSize * obstacle.ySpawnRange.x / 2, roadGenerator.chunkSize * obstacle.ySpawnRange.y / 2);
        Vector3 obstaclePos = new Vector3(chunkCoord.x * roadGenerator.chunkSize + chunkOffsetX, chunkCoord.y * roadGenerator.chunkSize + chunkOffsetY, 0);
        bool canSpawn = true;
        foreach (GameObject obs in spawnedObstacles)
        {
            if (Vector3.Distance(obs.transform.position, obstaclePos) < minimumObstacleDistance)
            {
                canSpawn = false;
            }
        }
        if (canSpawn)
        {
            GameObject obs;

            obs = Instantiate(obstacle.obstacleTemplate);
            obs.transform.position = obstaclePos;
            spawnedObstacles.Add(obs);
            /* (obstacle.shadowCast)
            {
                obs.AddComponent<ShadowScript>();
            }
            if (obstacle.fadeAnim)
            {
                obs.AddComponent<ObstacleCollisionEffect>();
            }
        }
    }

    List<GameObject> spawnedObstacles = new List<GameObject>();
    private void generateObstacles()
    {
        int spawnOffScreenChunkOffset = 6;
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject obs in spawnedObstacles)
        {
            if (Mathf.Abs((obs.transform.position.y / roadGenerator.chunkSize) - roadGenerator.currentViewerChunk.y) > spawnOffScreenChunkOffset + 1)
            {
                toRemove.Add(obs);
            }
            if (obs == null)
            {
                toRemove.Add(obs);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            spawnedObstacles.Remove(toRemove[i]);
            if (toRemove[i] != null)
            {
                Destroy(toRemove[i]);
            }

        }
        toRemove.Clear();


        int xOffsetSpawn = Mathf.RoundToInt(Random.Range(-0.6f, 0.6f)); //0.6 used to make more likely to spawn on road
                                                                        //handles road obstacles on the road itself 
        if (spawnedObstacles.Count < maxObstacleCount)
        {
            findObstacle(new Vector2(roadGenerator.currentViewerChunk.x + xOffsetSpawn, roadGenerator.currentViewerChunk.y + spawnOffScreenChunkOffset));
        }

        //handles obstacles on sholder
    }*/
}
