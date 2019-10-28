using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    // Start is called before the first frame update

    public void LoadLevel(string name)
    {
        string username = usernameText.text.Trim(new char[] { (char)8203 });
        if (username != "")
        {
            GameManager.Instance.m_PlayerUsername = username;
            SceneManager.LoadScene(name);
        }
        
    }
}
