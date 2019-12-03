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
                Player p = collision.gameObject.GetComponent<Player>();
                Player[] players = FindObjectsOfType<Player>();
                for(int i = 0; i < players.Length; i++) //remove all players except the winner for leaderboards
                {
                    if(players[i] != p)
                    {
                        GameManager.Instance.RemovePlayer(players[i].GetUsername());
                    }
                }
                PlayerWonMessage message = new PlayerWonMessage(p.GetUsername());
                VHostBehavior.Instance.SendMessageToAllPlayers(message);
                GameManager.Instance.PlayerHasWonGame(p);
            }
        }
    }
}
