using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempObstacleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("animate crash, drastically lower speed");
        }
        if(collision.GetComponent<ClientPlayer>())
        {
            collision.GetComponent<ClientPlayer>().playerInfo.currentSpeed *= 0.85f;
        }
    }
}
