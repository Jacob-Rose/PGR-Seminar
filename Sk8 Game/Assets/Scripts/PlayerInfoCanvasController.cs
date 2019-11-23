using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoCanvasController : MonoBehaviour
{
    Player p;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    private void Start()
    {
        p = GetComponentInParent<Player>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if(p is NetworkedPlayer)
        {
            nameText.text = (p as NetworkedPlayer).playerID;
        }
        else
        {
            nameText.text = GameManager.Instance.m_PlayerUsername;
        }
        scoreText.text = p.playerInfo.currentScore.ToString();
    }
}
