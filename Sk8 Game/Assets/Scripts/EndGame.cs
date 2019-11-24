using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (VHostBehavior.Instance != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Player p = collision.gameObject.GetComponent<ClientPlayer>();
                string playerID;
                if (p == null)
                {
                    p = collision.gameObject.GetComponent<NetworkedPlayer>();
                    playerID = (p as NetworkedPlayer).playerID;
                }
                else
                {
                    playerID = GameManager.Instance.m_PlayerUsername;
                }
                GameManager.Instance.PlayerHasWonGame(p);
                PlayerWonMessage message = new PlayerWonMessage(playerID);
            }
        }
    }
}
