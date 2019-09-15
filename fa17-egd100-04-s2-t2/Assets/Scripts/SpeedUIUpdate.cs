using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpeedUIUpdate : MonoBehaviour {

	public GameObject driverReference;

	void OnGUI()
	{
		gameObject.GetComponent<Text> ().text = (driverReference.GetComponent<DriverControl> ().getCarSpeed ()).ToString("#####");
	}
}
