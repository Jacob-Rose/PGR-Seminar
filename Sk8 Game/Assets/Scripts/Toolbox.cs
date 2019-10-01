﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Toolbox
{
    private bool m_GameStarted = false;
    private List<Listener> m_Listeners;

    private static Toolbox instance = null;

    public static Toolbox Instance { get
        {
            if(instance == null)
            {
                instance = new Toolbox();
            }
            return instance;
        }
    }
    public bool HasGameStarted { get { return m_GameStarted; } }
    public void StartGame()
    {
        //add any other things we need
        callListeners();
        m_GameStarted = true;
    }

    public void addListener(Listener l)
    {
        m_Listeners.Add(l);
    }

    private void callListeners()
    {
        for(int i =0; i < m_Listeners.Count; i++)
        {
            //m_Listeners[i].OnListenerCall();
        }
    }
}

public interface Listener
{
    //public abstract void OnListenerCall();
}