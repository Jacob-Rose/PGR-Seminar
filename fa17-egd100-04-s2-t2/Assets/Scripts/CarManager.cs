using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour {

	bool hasStopped;
    public GameObject carObject;

	public GameObject scoreKeeper;

	public float collisionPenalty;
	// Use this for initialization
	void Start () {
        carObject = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "StopSign")
        {
			if (carObject.GetComponent<DriverControl> ().getCarSpeed () == 0) {
				hasStopped = true;
			}
        }
    }
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.tag == "StopSign") {
			if (!hasStopped) {
				scoreKeeper.GetComponent<EventTracker>().score -= col.gameObject.GetComponent<ObstacleAttach> ().scoreReduction ;
			} else {
				
			}
		}
		hasStopped = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Colliders") {
			scoreKeeper.GetComponent<EventTracker> ().score -= col.gameObject.GetComponent<ObstacleAttach> ().scoreReduction;
			col.gameObject.tag = "Collided"; //easy way to track already collided
			if (col.gameObject.GetComponent<ObstacleCollisionEffect> () != null) {
				col.gameObject.GetComponent<ObstacleCollisionEffect> ().Collided = true;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		Debug.Log (col.gameObject.tag);
		if (col.gameObject.tag == "Colliders") {
			scoreKeeper.GetComponent<EventTracker> ().score -= col.gameObject.GetComponent<ObstacleAttach>().scoreReduction;
			col.gameObject.tag = "Collided"; //easy way to track already collided
			if (col.gameObject.GetComponent<ObstacleCollisionEffect> () != null) {
				col.gameObject.GetComponent<ObstacleCollisionEffect> ().Collided = true;
			}
			if (col.gameObject.GetComponent<ObstacleAttach> ().countsAsStrike) {
				scoreKeeper.GetComponent<EventTracker> ().addStrike ();
			}
		}
		if (col.gameObject.tag == "Bounds") {
			
			scoreKeeper.GetComponent<EventTracker> ().endGame ();
		}
	}
}
