﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    public string m_PlayerUsername;
    public Color m_PlayerColor = Color.white;
    public bool isDebug = false;
    public Player leadPlayer;
    public Player lastPlace;
    public float m_MusicVolumeOnPlay = 0.5f;
    public float maxDistanceToDQ = 10.0f;
    public List<Player> m_Players = new List<Player>();
    public List<Obstacle> m_AllObstacles = new List<Obstacle>();
    public Dictionary<string, long> m_DeletedPlayers = new Dictionary<string, long>(); //stores the tick when player was deleted, easy for ranking

    public bool HasGameStarted { get; private set; } = false;
    public bool HasGameEnded { get; private set; } = false;
    public float SecondsTillStart { get { return (float)(timeToStart - DateTime.UtcNow).TotalSeconds; } }

    public ClientPlayer ClientPlayer { get { return m_ClientPlayer; } }

    protected ClientPlayer m_ClientPlayer = null;
    protected DateTime timeToStart;
    protected bool m_GameIsStarting = false;

    public void Awake()
    {
        Instance = this;
    }

    public int getAllObstacleCount()
    {
        return m_AllObstacles.Count;
    }

    public bool HasPlayerExisted(string playerID)
    {
        long ticks;
        return m_DeletedPlayers.TryGetValue(playerID, out ticks);
    }

    public void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
        lastPlace = null;
        leadPlayer = null;
        DontDestroyOnLoad(this);
        SpawnClientPlayer();
    }

    public void PlayerAttacked(string playerID)
    {
        Player p = GetPlayer(playerID);
        p.m_PlayerInfo.currentScore -= 5;
        p.m_PlayerInfo.currentSpeed *= 0.90f;
        p.StartSpin();
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

        for (int i = 0; i < m_Players.Count; i++)
        {
            if (lastPlace == null || m_Players[i].transform.position.y < lastPlace.transform.position.y)
            {
                lastPlace = m_Players[i];
            }
            if (leadPlayer == null || m_Players[i].transform.position.y > leadPlayer.transform.position.y)
            {
                leadPlayer = m_Players[i];
            }
        }
        /*if(Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("Leaderboards");
        }*/
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
        if(m_ClientPlayer == null && !GameManager.Instance.HasGameEnded)
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
    }

    public void SpawnClientPlayer()
    {
        m_ClientPlayer = ((GameObject)Instantiate(Resources.Load("Prefabs/ClientPlayer"))).GetComponent<ClientPlayer>();
        m_Players.Add(m_ClientPlayer);
    }

    public void StartGame()
    {
        if(HasGameStarted == false)
        {
            HasGameStarted = true;
            m_GameIsStarting = false;
            if(m_Players.Count == 1)
            {
                isDebug = true;
            }
        }
    }
    public void PlayerHasWonGame(Player p)
    {
        if(!isDebug)
        {
            SceneManager.LoadScene("Leaderboards");
            HasGameEnded = true;
        }
    }

    public void StartGameInSeconds(float seconds)
    {
        SceneManager.LoadScene("thepark");
        GetComponent<AudioSource>().volume = m_MusicVolumeOnPlay;
        timeToStart = DateTime.UtcNow.AddSeconds(seconds);
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

    public NetworkedPlayer AddPlayer(string playerID, Color c)
    {
        GameObject obj = Instantiate((GameObject)Resources.Load("Prefabs/NetworkedPlayer"));
        obj.GetComponent<NetworkedPlayer>().playerID = playerID;
        obj.GetComponent<NetworkedPlayer>().color = c;
        m_Players.Add(obj.GetComponent<NetworkedPlayer>());
        return obj.GetComponent<NetworkedPlayer>();
    }

    public void RemovePlayer(string playerID)
    {
        bool destroyed = false;
        Player p = GetPlayer(playerID);
        if(p != null)
        {
            m_Players.Remove(p);
            Destroy(p.gameObject);
            destroyed = true;
        }
        if(HasGameStarted)
        {
            if (destroyed)
            {
                m_DeletedPlayers.Add(playerID, DateTime.UtcNow.Ticks);
            }
            if (m_Players.Count == 1)
            {
                PlayerHasWonGame(m_Players[0]);
            }
        }
    }

    public void UpdatePlayerInformation(ref PlayerInfo info, string playerID)
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i] is NetworkedPlayer && (m_Players[i] as NetworkedPlayer).playerID == playerID ||
                m_Players[i] is ClientPlayer && playerID == m_PlayerUsername)
            {
                m_Players[i].m_PlayerInfo = info;
                m_Players[i].SetPosition(info.position);
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
            if (m_Players[i] is NetworkedPlayer && (m_Players[i] as NetworkedPlayer).playerID == playerID ||
                m_Players[i] is ClientPlayer && playerID == m_PlayerUsername)
            {
                return m_Players[i];
            }
        }
        return null;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("MainMenu");
        if(VHostBehavior.Instance != null)
        {
            Destroy(VHostBehavior.Instance.gameObject);
        }
        if (VOnlinePlayer.Instance != null)
        {
            Destroy(VOnlinePlayer.Instance.gameObject);
        }
        HasGameStarted = false;
        HasGameEnded = false;
        m_AllObstacles.Clear();
        Player[] players = FindObjectsOfType<Player>();
        for(int i =0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }
        m_Players.Clear();
        m_DeletedPlayers.Clear();
        Destroy(gameObject);
    }

}