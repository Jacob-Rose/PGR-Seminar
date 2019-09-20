﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : Player
{
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    void updatePlayerInfo(PlayerInfo info)
    {
        posInfo.currentSpeed = info.currentSpeed;
        posInfo.zRot = info.zRot;
        posInfo.position = info.position;
        posInfo.move = info.move;
    }
}
