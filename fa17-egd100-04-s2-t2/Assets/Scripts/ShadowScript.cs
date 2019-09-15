using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShadowScript : MonoBehaviour {

	private GameObject shadow;

	public float shadowOffsetX = -1.0f;
	public float shadowOffsetY = -1.0f;

	// Use this for initialization
	void Start () {
		shadow = new GameObject ("Shadow");
		shadow.transform.parent = this.transform;
		shadow.transform.position = this.transform.position + new Vector3 (shadowOffsetX, shadowOffsetY);
		shadow.transform.rotation = this.transform.rotation;
		shadow.AddComponent<SpriteRenderer> ();
		shadow.transform.localScale = new Vector3 (1, 1, 1);
		shadow.GetComponent<SpriteRenderer> ().sprite = this.GetComponent<SpriteRenderer> ().sprite;
		shadow.GetComponent<SpriteRenderer> ().sortingLayerID = this.GetComponent<SpriteRenderer> ().sortingLayerID ;
		shadow.GetComponent<SpriteRenderer> ().color = new Color (0.0f, 0.0f, 0.0f, 0.5f);
		shadow.GetComponent<SpriteRenderer> ().sortingOrder = -50;
	}
	
	// Update is called once per frame
	void Update () {
		shadow.transform.position = this.transform.position + new Vector3 (shadowOffsetX, shadowOffsetY);
		shadow.transform.rotation = this.transform.rotation;
	}
}
