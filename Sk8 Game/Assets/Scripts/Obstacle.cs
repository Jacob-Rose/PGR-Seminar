using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour 
{
    public Sprite interactedSprite = null;
    public static List<Obstacle> m_AllObstacles = new List<Obstacle>();
    //public GameObject self;
    public uint id;
    public float speedMultiplier; //possibly add boost with two times
    public bool spinPlayer; //TODO implement
    public int scoreIncreaseOnInteract = 5;
    public Obstacle()
    {
        m_AllObstacles.Add(this);
    }

    ~Obstacle()
    {
        m_AllObstacles.Remove(this);
    }

    public void InteractedWith(Player p)
    {
        p.playerInfo.currentScore += scoreIncreaseOnInteract;
        if(interactedSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = interactedSprite;
        }
        
    }

    public void HandleInteraction(Player p)
    {
        if (p is ClientPlayer)
        {
            if (VHostBehavior.m_Instance != null)
            {
                VHostBehavior.m_Instance.SendMessageToAllPlayers(new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, id));
            }
            else if (VOnlinePlayer.m_Instance != null)
            {
                VOnlinePlayer.m_Instance.SendMessage(new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, id));
            }
        }
        InteractedWith(p);
    }

    public static int getAllObstacleCount()
    {
        return m_AllObstacles.Count;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Player>().playerInfo.collidable == true)
            {
                collision.gameObject.GetComponent<Player>().playerInfo.currentSpeed *= speedMultiplier;
                collision.gameObject.GetComponent<Player>().StartSpin();
                Debug.Log("ran spin");
            }
            else
            {
                collision.gameObject.GetComponent<ClientPlayer>().playerInfo.collidable = true;
            }
        }
    }


}
