using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreUIUpdate : MonoBehaviour {

	public GameObject scoreKeeper;

	void OnGUI()
	{
		gameObject.GetComponent<Text> ().text = scoreKeeper.GetComponent<EventTracker> ().score.ToString("######");
	}
}
