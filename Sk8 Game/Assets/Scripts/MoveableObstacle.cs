using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObstacle : IObstacle
{
    public Sprite interactedSprite = null;
    public override void HandleInteraction(Player p)
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

    public override void InteractedWith(Player p)
    {
        m_SpriteRenderer.sprite = interactedSprite;
        m_CanBeInteractedWith = false;
        GetComponent<Collider2D>().enabled = false;
        p.playerInfo.currentScore += scoreIncreaseOnInteract;
    }
}
