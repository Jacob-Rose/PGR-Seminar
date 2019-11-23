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
        m_Targets = GameManager.Instance.GetPlayers();
        if (m_Targets.Count == 0)
        {
            throw new System.Exception("No Players Exist");
        }
        Bounds newPos = getEncapsulatingBounds();
        transform.position = Vector3.Lerp(transform.position, new Vector3(0.0f, newPos.center.y, -10.0f), lerpAmount);
        m_Camera.orthographicSize = newPos.extents.y * zoomMultiplier + zoomOffset;

        Player lastPlace = null;
        Player firstPlace = null;
        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (lastPlace == null || m_Targets[i].transform.position.y < lastPlace.transform.position.y)
            {
                lastPlace = m_Targets[i];
            }
            if (firstPlace == null || m_Targets[i].transform.position.y > firstPlace.transform.position.y)
            {
                firstPlace = m_Targets[i];
            }
        }
        PostProcessVolume v = GetComponent<PostProcessVolume>();
        Vignette vig = v.profile.GetSetting<Vignette>();
        if(GameManager.Instance.ClientPlayer != null)
        {
            float distFromFirst = firstPlace.transform.position.y - GameManager.Instance.ClientPlayer.transform.position.y;
            vig.intensity.value = distFromFirst / GameManager.Instance.maxDistanceToDQ;
        }
        else
        {
            vig.intensity.value = 0.0f;
        }
        
        
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
