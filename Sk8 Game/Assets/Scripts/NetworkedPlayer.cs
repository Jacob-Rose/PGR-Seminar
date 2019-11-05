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

    public void OnGUI()
    {
        if(Camera.main != null)
        {
            Vector3 point = Camera.main.WorldToScreenPoint(transform.position);
            GUIStyle labelStyle = GUI.skin.label;
            labelStyle.fontSize = 12;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            Rect labelRect = new Rect(point + new Vector3(0, -10, 0), new Vector2(200.0f, 50.0f));
            GUI.Label(labelRect, playerID, labelStyle);
        }
    }
}
