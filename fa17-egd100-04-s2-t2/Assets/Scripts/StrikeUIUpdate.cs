using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrikeUIUpdate : MonoBehaviour {

	public GameObject tracker;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.GetComponent<Text> ().text = tracker.GetComponent<EventTracker>().getStrikes ().ToString();
	}
}
