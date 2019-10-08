using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    private List<Player> targets; //set in start to use toolbox
    public float smoothTime = 0.1f;
    public float zoomOffset = 0.0f; //additional or less zoom to add from start
    public float zoomMultiplier = 1.0f; //how much to zoom out or in

    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        targets = Toolbox.Instance.m_Players;
    }

    // Update is called once per frame
    void Update()
    {
        if(targets.Count == 0)
        {
            return;
        }
        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 newPos = getCenterPoint();
        //Vector3 toSet = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        transform.position = new Vector3(0.0f, newPos.y, -10.0f);
    }

    void Zoom()
    {
        if(targets.Count > 0)
        {
            GetComponent<Camera>().orthographicSize = (GetGreatestDistance() * zoomMultiplier) + zoomOffset;
        }
        
    }

    float GetGreatestDistance()
    {
        Bounds b = new Bounds(targets[0].transform.position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            b.Encapsulate(targets[i].transform.position);
        }

        return b.size.x;
    }
    Vector3 getCenterPoint()
    {
        if(targets.Count == 1)
        {
            return targets[0].transform.position;
        }
        Bounds b = new Bounds(targets[0].transform.position, Vector3.zero);
        for(int i =1; i< targets.Count; i++)
        {
            b.Encapsulate(targets[i].transform.position);
        }
        return b.center;
    }
}
