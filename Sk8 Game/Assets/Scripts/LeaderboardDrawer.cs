using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardDrawer : MonoBehaviour
{
    Texture2D emptyTex = Texture2D.blackTexture;
    public Button mainMenuButton;
    void Start()
    {
        mainMenuButton.onClick.AddListener(GameManager.Instance.ResetGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        var enumerator = GameManager.Instance.m_DeletedPlayers.GetEnumerator();
        int playerCount = GameManager.Instance.m_DeletedPlayers.Count + GameManager.Instance.m_Players.Count;
        float width = Screen.width * 0.6f;
        float height = Screen.height * 0.5f;
        GUI.BeginGroup(new Rect(Screen.width * 0.2f, Screen.height * 0.2f, width,  height));
        List<string> playersInOrder = new List<string>();
        while (enumerator.MoveNext())
        {
            bool added = false;
            for(int i = 0; i < playersInOrder.Count; i++)
            {
                if(GameManager.Instance.m_DeletedPlayers[playersInOrder[i]] < enumerator.Current.Value)
                {
                    playersInOrder.Insert(i, enumerator.Current.Key);
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                playersInOrder.Insert(playersInOrder.Count, enumerator.Current.Key);
            }
        }

        for(int i = 0; i<GameManager.Instance.m_Players.Count; i++)
        {
            playersInOrder.Insert(0, GameManager.Instance.m_Players[i].GetUsername());
        }

        for(int i = 0; i < playersInOrder.Count; i++)
        {
            GUI.BeginGroup(new Rect(0, (height / playerCount) * i, width, height / playerCount));
            GUI.Box(new Rect(0, 0, width / 2, height / playerCount),emptyTex);
            GUI.Label(new Rect(0, 0, width / 2, height / playerCount), (i+1).ToString() + " " + playersInOrder[i]);
            GUI.EndGroup();
        }
        GUI.EndGroup();
    }
}
