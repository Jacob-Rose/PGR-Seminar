using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class VServerUIHandler : MonoBehaviour
{

    public TMPro.TextMeshProUGUI ipText;
    // Start is called before the first frame update
    void Start()
    {
        ipText.text = VServerBehavior.m_Instance.getIPString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
