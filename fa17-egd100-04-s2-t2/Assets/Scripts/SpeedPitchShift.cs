using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPitchShift : MonoBehaviour {

    public Vector2 range;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		this.GetComponent<AudioSource> ().pitch = ((range.y - range.x) * (this.GetComponent<DriverControl> ().getCarSpeed() / this.GetComponent<DriverControl> ().maxSpeed)) + range.x;
	}
}
