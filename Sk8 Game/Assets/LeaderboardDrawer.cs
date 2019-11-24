using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardDrawer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        var enumerator = GameManager.Instance.m_DeletedPlayers.GetEnumerator();
        Rect lineRect = new Rect(Screen.width * 0.1f, Screen.height * 0.4f, Screen.width * 0.8f, Screen.height * 0.2f);
        Texture2D gTxtre = new Texture2D(1, 1);
        gTxtre.SetPixel(0, 0, Color.green);
        gTxtre.wrapMode = TextureWrapMode.Repeat;
        GUI.DrawTexture(lineRect, gTxtre);
    }
}
