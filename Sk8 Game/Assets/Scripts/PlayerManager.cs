using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager
{
    public List<Player> m_Players;
    public ClientPlayer cPlayerRef;
    public NetworkedPlayer nPlayerRef;
    public void AddNetworkedPlayer(PlayerInfo info)
    {
        NetworkedPlayer player = UnityEngine.Object.Instantiate(nPlayerRef.gameObject).GetComponent<NetworkedPlayer>();
        m_Players.Add(player);
        player.updatePlayerInfo(info);
    }

    public void AddClientPlayer()
    {
        ClientPlayer player = UnityEngine.Object.Instantiate(cPlayerRef.gameObject).GetComponent<ClientPlayer>();
        m_Players.Add(player);
    }

    public void HandlePlayerUpdate(PlayerInfo info)
    {
        for(int i =0; i < m_Players.Count; i++)
        {
            //ignore all other players with continue
            if (m_Players[i].GetPlayerInfo().id != info.id)
                continue;

            ((NetworkedPlayer)m_Players[i]).updatePlayerInfo(info);
        }
    }

}