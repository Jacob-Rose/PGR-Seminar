using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ConnectionAutoFiller : MonoBehaviour
{
    public string[] lastIPs = new string[5];
    public TMP_InputField textBox;
    public Button connectButton;
    public string autofillString = "";
    // Start is called before the first frame update
    void Start()
    {
        connectButton.onClick.AddListener(ConnectPressed);
        for(int i = 0; i < 5; i++)
        {
            if (!PlayerPrefs.HasKey("lastIP" + i.ToString()))
            {
                PlayerPrefs.SetString("lastIP" + i.ToString(), "");
            }
        }
        LoadStrings();
    }

    void ConnectPressed()
    {
        for(int i = 0; i < 5; i++)
        {
            if(lastIPs[i] == textBox.text)
            {
                return;
            }
        }
        for (int i = 4; i > 0; i--)
        {
            lastIPs[i] = lastIPs[i - 1];
        }
        lastIPs[0] = textBox.text;
        SaveStrings();
    }

    void SaveStrings()
    {
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetString("lastIP" + i.ToString(), lastIPs[i]);
        }
    }

    void LoadStrings()
    {
        for (int i = 0; i < 5; i++)
        {
            lastIPs[i] = PlayerPrefs.GetString("lastIP" + i.ToString());
        }
    }
    private void OnGUI()
    {
        int validCount = 0;
        foreach(string ip in lastIPs)
        {
            if(ip != "")
            {
                validCount++;
            }
        }
        for (int i = 0; i < validCount; i++)
        {
            if(lastIPs[i] != "")
            {
                if (GUI.Button(new Rect(Screen.width * (((float)i) / validCount), Screen.height * 0.8f, 150, 40), lastIPs[i]))
                {
                    textBox.text = lastIPs[i];
                }
            }
            else
            {
                i--;
            }
        }
    }
}
