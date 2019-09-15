using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMenuManager : MonoBehaviour
{
    public ClientBehavior m_Client;
    public TMPro.TMP_InputField m_IPInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConnectToServer()
    {
        m_Client.ConnectToIP(m_IPInput.text);
    }
}
