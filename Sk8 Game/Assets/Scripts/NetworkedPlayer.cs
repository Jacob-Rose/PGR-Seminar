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
    }

    public void updatePlayerInfo(PlayerInfo info)
    {
        playerInfo = info;
    }
}
