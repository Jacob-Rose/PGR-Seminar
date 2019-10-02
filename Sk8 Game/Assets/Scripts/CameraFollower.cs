using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    public List<Player> targets;
    public float smoothTime = 0.5f;

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
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    void Zoom()
    {
        if(targets.Count > 1)
        {
            GetComponent<Camera>().fieldOfView = GetGreatestDistance();
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
