﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkedPlayer : Player
{
    public string playerID = "";
    public Color color = Color.white;

    public override string GetUsername()
    {
        return playerID;
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
