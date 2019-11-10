using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour 
{
    public Sprite defaultSprite = null; //used to fix bug
    public uint id;
    public float speedMultiplier; 
    public bool m_ShouldSpinPlayer;

    protected SpriteRenderer m_SpriteRenderer;

    protected virtual void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = defaultSprite;
    }

    protected virtual void OnDestroy()
    {
        if(GameManager.Instance.m_AllObstacles.Contains(this))
        {
            GameManager.Instance.m_AllObstacles.Remove(this);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (p.playerInfo.collidable == true && !p.m_IsSpinning)
            {
                p.playerInfo.currentSpeed *= speedMultiplier;
                if(m_ShouldSpinPlayer)
                {
                    p.StartSpin();
                }
            }
            else
            {
                collision.gameObject.GetComponent<Player>().playerInfo.collidable = true;
            }
        }
    }


}
