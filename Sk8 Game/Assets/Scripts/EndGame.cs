using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<ClientPlayer>();
            if(p == null)
            {
                p = collision.gameObject.GetComponent<NetworkedPlayer>();
            }
            GameManager.Instance.PlayerHasWonGame(p);
        }
    }
}
