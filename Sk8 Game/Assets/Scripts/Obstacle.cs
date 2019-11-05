using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour 
{
    public Sprite defaultSprite = null;
    public Sprite interactedSprite = null;
    public static List<Obstacle> m_AllObstacles = new List<Obstacle>();
    //public GameObject self;
    public uint id;
    public float speedMultiplier; //possibly add boost with two times
    public bool spinPlayer; //TODO implement
    public int scoreIncreaseOnInteract = 5;

    private SpriteRenderer m_SpriteRenderer;
    public Obstacle()
    {
        m_AllObstacles.Add(this);
    }

    ~Obstacle()
    {
        m_AllObstacles.Remove(this);
    }

    public void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = defaultSprite;
    }

    private void OnDestroy()
    {
        m_AllObstacles.Remove(this);
    }

    public void InteractedWith(Player p)
    {
        p.playerInfo.currentScore += scoreIncreaseOnInteract;
        if(interactedSprite != null)
        {
            m_SpriteRenderer.sprite = interactedSprite;
        }
        
    }

    public void HandleInteraction(Player p)
    {
        if (p is ClientPlayer)
        {
            if (VHostBehavior.Instance != null)
            {
                VHostBehavior.Instance.SendMessageToAllPlayers(new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, id));
            }
            else if (VOnlinePlayer.Instance != null)
            {
                VOnlinePlayer.Instance.SendMessage(new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, id));
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
            Player p = collision.gameObject.GetComponent<Player>();
            if (p.playerInfo.collidable == true && !p.m_IsSpinning)
            {
                p.playerInfo.currentSpeed *= speedMultiplier;
                p.StartSpin();
                Debug.Log("ran spin");

            }
            else
            {
                collision.gameObject.GetComponent<Player>().playerInfo.collidable = true;
            }
        }
    }


}
