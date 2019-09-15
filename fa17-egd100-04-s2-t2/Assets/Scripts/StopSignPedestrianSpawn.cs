using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignPedestrianSpawn : MonoBehaviour {

	public float pedSpawnChance;
	public GameObject pedestrian;

	// Use this for initialization
	void Start () {
		if (Random.value < pedSpawnChance) {
			GameObject ped= Instantiate (pedestrian);
			ped.transform.position = this.transform.position + new Vector3 (5, 10, 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
