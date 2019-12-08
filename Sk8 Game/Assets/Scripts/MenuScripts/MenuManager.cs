using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public int usernameCharLimit = 16;
    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(PlayerSettings.defaultScreenWidth, PlayerSettings.defaultScreenHeight, PlayerSettings.fullScreenMode);
    }
    public void LoadLevel(string name)
    {
        string username = usernameText.text.Trim(new char[] { (char)8203 });
        if (username != "")
        {
            if(username.Length > usernameCharLimit)
            {
                username.Substring(0, usernameCharLimit);
            }
            GameManager.Instance.m_PlayerUsername = username;
            SceneManager.LoadScene(name);
        }
        
    }
}
