using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour 
{
    public Sprite defaultSprite = null;
    public Sprite interactedSprite = null;
    //public GameObject self;
    public uint id;
    public float speedMultiplier; //possibly add boost with two times
    public bool spinPlayer; //TODO implement
    public int scoreIncreaseOnInteract = 5;

    public bool m_InteractedWith = false;

    private SpriteRenderer m_SpriteRenderer;

    public virtual void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = defaultSprite;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance.m_AllObstacles.Contains(this))
        {
            GameManager.Instance.m_AllObstacles.Remove(this);
        }
        
    }

    public void InteractedWith(Player p)
    {
        m_SpriteRenderer.sprite = interactedSprite;
        m_InteractedWith = true;
        p.playerInfo.currentScore += scoreIncreaseOnInteract;
    }

    public void HandleInteraction(Player p)
    {
        if (p is ClientPlayer)
        {
            if (VHostBehavior.Instance != null)
            {
                VHostBehavior.Instance.SendMessageToAllPlayers(new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, id), Valve.Sockets.SendType.Reliable);
            }
            else if (VOnlinePlayer.Instance != null)
            {
                VOnlinePlayer.Instance.SendMessage(new ObstacleModifiedMessage(GameManager.Instance.m_PlayerUsername, id));
            }
        }
        InteractedWith(p);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !m_InteractedWith)
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
