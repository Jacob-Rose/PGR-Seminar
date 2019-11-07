using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    private List<Player> m_Targets; //set in start to use toolbox
    private Camera m_Camera;
    [Range(0.0f,1.0f)]
    public float lerpAmount = 0.6f;
    public float zoomOffset = 0.0f; //additional or less zoom to add from start
    public float zoomMultiplier = 1.0f; //how much to zoom out or in

    public float maxDistance = 10.0f;

    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        m_Targets = GameManager.Instance.GetPlayers();
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Targets.Count == 0)
        {
            throw new System.Exception("No Players Exist");
        }
        m_Targets = GameManager.Instance.GetPlayers();
        if (m_Targets.Count == 0)
        {
            return;
        }
        Bounds newPos = getEncapsulatingBounds();

        //Vector3 toSet = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        transform.position = Vector3.Lerp(transform.position, new Vector3(0.0f, newPos.center.y, -10.0f), lerpAmount);
        m_Camera.orthographicSize = newPos.extents.y * zoomMultiplier + zoomOffset;

        Player lastPlace = null;
        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (lastPlace == null || m_Targets[i].transform.position.y < lastPlace.transform.position.y)
            {
                lastPlace = m_Targets[i];
            }
        }
        if(lastPlace is ClientPlayer)
        {
            PostProcessVolume v = GetComponent<PostProcessVolume>();
            Vignette vig;
            v.profile.TryGetSettings<Vignette>(out vig);
            vig.intensity.value = newPos.extents.y / maxDistance;
        }
        
        if (newPos.extents.y > maxDistance)
        {
            if (lastPlace is NetworkedPlayer)
            {
                GameManager.Instance.PlayerFellBehind((lastPlace as NetworkedPlayer).playerID);
            }
            else
            {
                GameManager.Instance.PlayerFellBehind(GameManager.Instance.m_PlayerUsername);
            }
        }
    }


    float GetGreatestDistance()
    {
        Bounds b = new Bounds(m_Targets[0].transform.position, Vector3.zero);
        for (int i = 1; i < m_Targets.Count; i++)
        {
            b.Encapsulate(m_Targets[i].transform.position);
        }

        return b.size.y;
    }
    Vector3 getCenterPoint()
    {
        if(m_Targets.Count == 1)
        {
            return m_Targets[0].transform.position;
        }
        Bounds b = new Bounds(m_Targets[0].transform.position, Vector3.zero);
        for(int i =0; i< m_Targets.Count; i++)
        {
            b.Encapsulate(m_Targets[i].transform.position);
        }
        return b.center;
    }

    Bounds getEncapsulatingBounds()
    {
        Bounds b = new Bounds(m_Targets[0].transform.position, Vector3.zero);
        for (int i = 1; i < m_Targets.Count; i++)
        {
            b.Encapsulate(m_Targets[i].transform.position);
        }
        return b;
    }
}
