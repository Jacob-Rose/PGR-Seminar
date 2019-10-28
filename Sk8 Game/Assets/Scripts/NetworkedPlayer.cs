using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : Player
{
    public string playerID = "";

    public NetworkedPlayer(string id)
    {
        playerID = id;
    }

    public void updatePlayerInfo(PlayerInfo info)
    {
        playerInfo = info;
    }
}
