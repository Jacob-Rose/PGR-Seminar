using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkLoadManager : MonoBehaviour
{
    public Transform startPos;
    public float startWidth = 6.0f;
    // Start is called before the first frame update
    void Start()
    {
        List<Player> players = GameManager.Instance.GetPlayers();
        float offsetInc = (startWidth / 2.0f) / players.Count;
        for (int i = 0; i < players.Count; i++)
        {
            Vector3 offset = new Vector3((i * offsetInc) - (startWidth * 0.5f), 0, 0);
            players[i].SetPosition(new Vector3(startPos.position.x, startPos.position.y, 0.0f) + offset);
        }
    }
}
