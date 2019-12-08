using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardDrawer : MonoBehaviour
{
    Texture2D emptyTex;
    public Button mainMenuButton;
    List<string> playersInOrder = new List<string>();

    public TextMeshProUGUI[] positionTextBars;
    public string[] minecraftSpashTextOptions;
    public TextMeshProUGUI splashText;


    void Start()
    {
        mainMenuButton.onClick.AddListener(GameManager.Instance.ResetGame);
        emptyTex = Texture2D.blackTexture;
        GetPlayersInOrder();
        splashText.text = minecraftSpashTextOptions[UnityEngine.Random.Range(0, minecraftSpashTextOptions.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetPlayersInOrder()
    {
        var enumerator = GameManager.Instance.m_DeletedPlayers.GetEnumerator();
        int playerCount = GameManager.Instance.m_DeletedPlayers.Count + GameManager.Instance.m_Players.Count;
        float width = Screen.width * 0.6f;
        float height = Screen.height * 0.5f;
        GUI.BeginGroup(new Rect(Screen.width * 0.2f, Screen.height * 0.2f, width, height));
        while (enumerator.MoveNext())
        {
            bool added = false;
            for (int i = 0; i < playersInOrder.Count; i++)
            {
                if (GameManager.Instance.m_DeletedPlayers[playersInOrder[i]] < enumerator.Current.Value)
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

        for (int i = 0; i < GameManager.Instance.m_Players.Count; i++)
        {
            playersInOrder.Insert(0, GameManager.Instance.m_Players[i].GetUsername());
        }
    }

    private void OnGUI()
    {
        for(int i = 0; i < positionTextBars.Length; i++)
        {
            if(playersInOrder.Count > i)
            {
                positionTextBars[i].text = playersInOrder[i];
            }
            else
            {
                positionTextBars[i].text = "";
            }
        }
    }
}
