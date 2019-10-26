using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public List<Player> m_Players;
    public ClientPlayer cPlayerRef;
    public NetworkedPlayer nPlayerRef;

    private ClientPlayer mClientPlayer;

    public void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void AddNetworkedPlayer(PlayerInfo info)
    {
        NetworkedPlayer player = Instantiate(nPlayerRef.gameObject).GetComponent<NetworkedPlayer>();
        player.transform.position = new Vector3(m_Players.Count * 4.0f, 0.0f);
        m_Players.Add(player);
        player.updatePlayerInfo(info);
    }

    public ClientPlayer getClientPlayer()
    {
        return mClientPlayer;
    }
    public void AddClientPlayer()
    {
        ClientPlayer player = Instantiate(cPlayerRef.gameObject).GetComponent<ClientPlayer>();
        m_Players.Add(player);
        player.transform.position = new Vector3(m_Players.Count * 4.0f, 0.0f);
        mClientPlayer = player;
    }

    public void HandlePlayerUpdate(PlayerInfo info)
    {
        /*
        for(int i =0; i < m_Players.Count; i++)
        {
            //ignore all other players with continue
            if (m_Players[i].GetPlayerInfo().id != info.id)
                continue;

            ((NetworkedPlayer)m_Players[i]).updatePlayerInfo(info);
        }
        */
    }

}