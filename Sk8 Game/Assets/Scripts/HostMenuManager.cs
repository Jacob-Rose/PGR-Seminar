using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostMenuManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI ipText;
    // Start is called before the first frame update
    void Start()
    {
        ipText.text = Networked.GetIP().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
