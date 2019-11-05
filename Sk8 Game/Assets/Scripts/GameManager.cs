using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameManager : MonoBehaviour
{
    private bool m_GameStarted = false;

    private DateTime timeToStart;
    private bool m_GameIsStarting = false;

    private static GameManager instance = null;

    public static GameManager Instance { get { return instance; } }

    public List<Player> m_Players = new List<Player>();

    public bool HasGameStarted { get { return m_GameStarted; } }
    public float SecondsTillStart { get { return (float)(timeToStart - DateTime.Now).TotalSeconds; } }

    public ClientPlayer ClientPlayer { get { return m_ClientPlayer; } }

    protected ClientPlayer m_ClientPlayer = null; //all networked will have a single client player that sends data
    public string m_PlayerUsername;



    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        DontDestroyOnLoad(this);
        SpawnClientPlayer();
    }

    public void Update()
    {
        if(m_GameIsStarting)
        {
            if(SecondsTillStart < 0.0f)
            {
                StartGame();
            }
        }
    }

    public void OnGUI()
    {
        if(m_GameIsStarting)
        {
            GUIStyle textStyle = GUI.skin.label;
            textStyle.alignment = TextAnchor.MiddleCenter;
            GUI.color = Color.red;
            textStyle.fontSize = 40;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height /2), SecondsTillStart.ToString("F1"), textStyle);
        }
        if(m_ClientPlayer == null)
        {
            GUIStyle textStyle = GUI.skin.label;
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.fontSize = 30;
            GUI.color = Color.red;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height / 2), "Spectating", textStyle);
        }
    }

    public void PlayerFellBehind(string playerID)
    {
        RemovePlayer(playerID);
        if(VHostBehavior.m_Instance != null)
        {
            VHostBehavior.m_Instance.SendMessageToAllPlayers(new PlayerFellBehindMessage(playerID));
        }
    }

    public void SpawnClientPlayer()
    {
        m_ClientPlayer = ((GameObject)Instantiate(Resources.Load("Prefabs/ClientPlayer"))).GetComponent<ClientPlayer>();
        m_Players.Add(m_ClientPlayer);
    }

    public void StartGame()
    {
        if(m_GameStarted == false)
        {
            m_GameStarted = true;
            m_GameIsStarting = false;
        }
    }

    public void StartGameInSeconds(float seconds)
    {
        SceneManager.LoadScene("thepark");
        timeToStart = DateTime.Now.AddSeconds(seconds);
        m_GameIsStarting = true;
        Invoke("StartGame", seconds);
    }

    public NetworkedPlayer AddPlayer(string playerID)
    {
        GameObject obj = Instantiate((GameObject)Resources.Load("Prefabs/NetworkedPlayer"));
        obj.GetComponent<NetworkedPlayer>().playerID = playerID;
        m_Players.Add(obj.GetComponent<NetworkedPlayer>());
        return obj.GetComponent<NetworkedPlayer>();
    }

    public void ObstacleInteractedWith(uint obstacleID, string playerID)
    {
        Obstacle.m_AllObstacles[(int)obstacleID].InteractedWith(GetPlayer(playerID));
    }

    public void RemovePlayer(string playerID)
    {
        if(playerID == this.m_PlayerUsername)
        {
            Destroy(m_Players[0].gameObject);
            m_Players.RemoveAt(0);
        }
        else
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                NetworkedPlayer nPlayer = m_Players[i].GetComponent<NetworkedPlayer>();
                if (nPlayer != null && nPlayer.playerID == playerID)
                {
                    Destroy(m_Players[i].gameObject);
                    m_Players.RemoveAt(i);
                }
            }
        }

        if(m_Players.Count == 1)
        {
            PlayerHasWonGame(m_Players[0]);
        }
    }

    public void PlayerHasWonGame(Player p)
    {

    }

    public void UpdatePlayerInformation(ref PlayerInfo info, string playerID)
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            NetworkedPlayer nPlayer = m_Players[i].GetComponent<NetworkedPlayer>();
            if (nPlayer != null && nPlayer.playerID == playerID)
            {
                nPlayer.playerInfo = info;
            }
        }
    }

    public List<Player> GetPlayers()
    {
        return m_Players;
    }

    public Player GetPlayer(string playerID)
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            NetworkedPlayer nPlayer = m_Players[i].GetComponent<NetworkedPlayer>();
            if (nPlayer != null && nPlayer.playerID == playerID)
            {
                return nPlayer;
            }
        }
        return null;
    }

}