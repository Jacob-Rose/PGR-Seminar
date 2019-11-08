﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public float speedMultiplier = 0.9f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.playerInfo.currentSpeed = Mathf.Lerp(p.playerInfo.currentSpeed, p.MaxSpeed * speedMultiplier, (1.0f - Time.deltaTime));
        }
    }
}
