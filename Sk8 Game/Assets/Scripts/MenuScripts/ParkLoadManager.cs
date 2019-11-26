using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParkLoadManager : MonoBehaviour
{

    // Start is called before the first frame update
    private void OnGUI()
    {
        InputDevice device = GameManager.Instance.ClientPlayer.m_LatestDevice;
        if (device != null)
        {
            if(device.name.Contains("Keyboard"))
            {

            }
            else if(device.name.Contains("Dualshock4"))
            {

            }
        }
    }
}
