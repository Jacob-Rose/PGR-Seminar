using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DarkRift.Client.DarkRiftClient client;

    public enum EventType
    {
        PlayerMoveEvent,
        PlayerInteractEvent,
        TileLoadEvent,
    }

    private void Awake()
    {
        client.MessageReceived += MessageReceived;
    }

    public void MessageReceived(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
        DarkRift.Message msg = e.GetMessage();
        if(msg.Tag.Equals(EventType.PlayerMoveEvent))
        {

        }
    }
}
