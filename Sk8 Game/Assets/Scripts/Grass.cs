using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public float maxSpeedRelative = 0.6f;
    public float speedMultiplier = 0.9f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<ClientPlayer>().playerInfo.currentSpeed *= speedMultiplier * (1.0f-Time.deltaTime);
        }
    }
}
