using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverControl : MonoBehaviour {

    private float carSpeed;

    public float turnMultiplier;
	public float accelerationMultiplier;
	public float brakingMultiplier;

	public float maxSpeed;

	[Range(1,60)]
	public int accelerationDelayAmount;
	private List<float> accelerationInputListHolder = new List<float>(); //used for delay timer

    [Range(1,60)]
    public int turnDelayAmount;
	private List<float> turnInputListHolder = new List<float>(); //used for delay timer

    [Range(1,1.2f)]
    public float turnRotationDrag; //controls turn slowdown

    public GameObject carObject;

    public float maxTurnSpeed;

    private float rotationSpeed = 0;

	public float turnBias;


    void Start () {
        for(int i = 0; i < turnDelayAmount; i++)
        {
            turnInputListHolder.Add(0.0f);
        }
		for(int i = 0; i < accelerationDelayAmount; i++)
		{
			accelerationInputListHolder.Add(0.0f);
		}
    }

	public float getCarSpeed()
	{
		return carSpeed;
	}
	// Update is called once per frame
	void Update () {
        #region Input Cue
        float turnInput = turnInputListHolder[0]; //get float from cue
        turnInputListHolder.RemoveAt(0); //remove value from cue
        turnInputListHolder.Insert(turnInputListHolder.Count, (Input.GetAxis("carTurn"))); //add current input to cue
		float accelerationInput = accelerationInputListHolder[0]; //get float from cue
		accelerationInputListHolder.RemoveAt(0); //remove value from cue
		accelerationInputListHolder.Insert(accelerationInputListHolder.Count, (Input.GetAxis("carAcceleration"))); //add current input to cue
        #endregion

		#region Rotation Handler
		rotationSpeed += ((turnInput+turnBias) * Time.deltaTime * turnMultiplier)* (carSpeed / 70); 
        if(rotationSpeed > maxTurnSpeed)
        {
            rotationSpeed = maxTurnSpeed;
        }
        else if (rotationSpeed < -maxTurnSpeed)
        {
            rotationSpeed = -maxTurnSpeed;
        }
        rotationSpeed = rotationSpeed / (turnRotationDrag); //division probably not best method, but one I thought of

        Quaternion rotation = carObject.transform.rotation; //save rotation of object
        carObject.transform.rotation = rotation * Quaternion.AngleAxis(rotationSpeed, Vector3.back);
		#endregion

		if (accelerationInput > 0) {
			carSpeed += accelerationInput * Time.deltaTime * accelerationMultiplier;
		} else {
			carSpeed += accelerationInput * Time.deltaTime * brakingMultiplier;
		}
		if (carSpeed < 0) {
			carSpeed = 0;
		}
		if (carSpeed > maxSpeed) {
			carSpeed = maxSpeed;
		}
		Vector3 speedAdd = carObject.transform.up * Time.deltaTime * carSpeed * 100;
		carObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speedAdd.x, speedAdd.y) ;
		

        #region Delay Setting Updater
        if (turnDelayAmount > turnInputListHolder.Count-1) //fixes delay setting when value changed
        {
            for(int i = 0; i < turnDelayAmount - turnInputListHolder.Count; i++)
            {
                turnInputListHolder.Add(Input.GetAxis("carTurn")); //adds the current input value
            }
        }
        if (turnDelayAmount < turnInputListHolder.Count-1)
        {
            for (int i = 0; i < turnInputListHolder.Count - turnDelayAmount; i++)
            {
                turnInputListHolder.RemoveAt(0);
            }
        }
		if (accelerationDelayAmount > accelerationInputListHolder.Count-1) //fixes delay setting when value changed
		{
			for(int i = 0; i < accelerationDelayAmount - accelerationInputListHolder.Count; i++)
			{
				accelerationInputListHolder.Add(Input.GetAxis("carTurn")); //adds the current input value
			}
		}
		if (accelerationDelayAmount < accelerationInputListHolder.Count-1)
		{
			for (int i = 0; i < accelerationInputListHolder.Count - accelerationDelayAmount; i++)
			{
				accelerationInputListHolder.RemoveAt(0);
			}
		}
        #endregion
        
    }
}
