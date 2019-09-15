using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarObstacle : MonoBehaviour {

	public float carSpeed;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		Vector3 speed = this.transform.up * Time.deltaTime * carSpeed*100;
		this.GetComponent<Rigidbody2D>().velocity = new Vector2(speed.x, speed.y) ;
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		carSpeed = 0;
	}
}
