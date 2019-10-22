using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameManager : MonoBehaviour
{
    public void Start()
    {
        Toolbox.Instance.StartGame();
    }
}