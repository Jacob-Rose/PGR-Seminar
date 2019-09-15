using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollisionEffect : MonoBehaviour {
	[Range(0, 0.1f)]
	public float removeFadeSpeed = 0.01f;

	public bool Collided {
		get {
			return hasCollided;
		}

		set {
			if (value) {
				OnCollide ();
			}
			hasCollided = value;
		}
	}
	private bool hasCollided;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Collided) {
			Color col = this.gameObject.GetComponent<Renderer>().material.color ;
			col.a -= removeFadeSpeed;
			this.gameObject.GetComponent<Renderer>().material.color = col;
			if (this.GetComponent<SpriteRenderer> ().color.a < 0) {
				Destroy (this);
			}
		}
	}

	void OnCollide()
	{
		Destroy (this.GetComponent<BoxCollider2D> ());
	}
}
