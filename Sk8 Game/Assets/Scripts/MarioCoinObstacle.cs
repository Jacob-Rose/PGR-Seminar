using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioCoinObstacle : IObstacle
{
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
        p.m_PlayerInfo.currentScore += scoreIncreaseOnInteract;
        Destroy(gameObject);
    }
    // Start is called before the first frame update
}
