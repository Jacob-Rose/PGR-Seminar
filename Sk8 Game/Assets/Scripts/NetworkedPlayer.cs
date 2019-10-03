using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : Player
{
    public override void Update()
    {
        base.Update();
    }
    public void updatePlayerInfo(PlayerInfo info)
    {
        playerInfo = info;
    }
}
