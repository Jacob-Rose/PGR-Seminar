using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkedPlayer : Player
{
    public string playerID = "";

    public NetworkedPlayer(string id)
    {
        playerID = id;
    }
    public override void Start()
    {
        base.Start();
        GetComponentInChildren<TextMeshProUGUI>().text = playerID;
    }

    public void updatePlayerInfo(PlayerInfo info)
    {
        playerInfo = info;
    }
}
