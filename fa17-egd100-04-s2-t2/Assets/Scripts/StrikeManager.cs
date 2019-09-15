using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrikeManager : MonoBehaviour {

	public Image strike1;
	public Image strike2;
	public Image strike3;
	public EventTracker tracker;

	private Color strikeOnColor = new Color(1,1,1,1);
	private Color strikeOffColor = new Color(0.1f,0.1f,0.1f,0.7f);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (tracker.getStrikes () > 0) {
			strike1.color = strikeOffColor;
		}
		if (tracker.getStrikes () > 1) {
			strike2.color = strikeOffColor;
		}
		if (tracker.getStrikes () > 2) {
			strike3.color = strikeOffColor;
		}
	}
}
