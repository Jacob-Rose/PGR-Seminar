using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour 
{
    public static List<Obstacle> m_AllObstacles = new List<Obstacle>();
    public uint id;
    public float speedMultiplier; //possibly add boost with two times
    public bool spinPlayer; //TODO implement

    public void Start()
    {
        m_AllObstacles.Add(this);
    }

    public static int getAllObstacleCount()
    {
        return m_AllObstacles.Count;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<ClientPlayer>().playerInfo.currentSpeed *= speedMultiplier;
        }
    }

}
