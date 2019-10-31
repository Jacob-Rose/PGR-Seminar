using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkLoadManager : MonoBehaviour
{
    public Transform startPos;
    // Start is called before the first frame update
    void Start()
    {
        List<Player> players = GameManager.Instance.GetPlayers();
        foreach(Player p in players)
        {
            p.SetPosition(new Vector3(startPos.position.x, startPos.position.y, 0.0f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
