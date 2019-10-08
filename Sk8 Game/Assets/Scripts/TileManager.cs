using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject obstManager;
    public GameObject testObstacle;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetObstStatsSpawn", 2.0f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void GetObstStatsSpawn()
    {
        Vector3 obstaclePos = obstManager.GetComponent<ObstGen>().CreateObstaclePoint();
        Instantiate(testObstacle, obstaclePos, Quaternion.identity);
    }
}
