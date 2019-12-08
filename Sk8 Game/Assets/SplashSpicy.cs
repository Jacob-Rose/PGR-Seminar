using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashSpicy : MonoBehaviour
{
    public float scaleMultiplier = 1.0f;
    public float scaleChangeRate = 0.75f;
    public float maxSize = 1.25f;
    public float minSize = 0.75f;
    bool goingUp = true;

    // Update is called once per frame
    void Update()
    {
        if(goingUp)
        {
            scaleMultiplier += (scaleChangeRate * Time.deltaTime);
            if(scaleMultiplier > maxSize)
            {
                goingUp = false;
            }
        }
        else
        {
            scaleMultiplier -= (scaleChangeRate * Time.deltaTime);
            if (scaleMultiplier < minSize)
            {
                goingUp = true;
            }
        }
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
    }
}
