using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour {

    public GameObject followObject;

    public enum CameraMode { RotateWorld, RotateLock };

    public CameraMode cameraMode;

    public float cameraOffsetX;
    public float cameraOffsetY;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if(cameraMode == CameraMode.RotateWorld)
        {
            gameObject.transform.position = new Vector3(followObject.transform.position.x, followObject.transform.position.y, -10) + (followObject.transform.right * cameraOffsetY) + (followObject.transform.up * cameraOffsetY);

            gameObject.transform.rotation = followObject.transform.rotation;
        }
        else
        {
            gameObject.transform.position = new Vector3(followObject.transform.position.x + cameraOffsetX, followObject.transform.position.y + cameraOffsetY, -10);
        }
	}

}
