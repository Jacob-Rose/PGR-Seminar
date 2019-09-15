using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventTracker : MonoBehaviour {

	public float score;
	public float scoreMultiplier;
	public int stage = 0;
	private bool levelUp = true;

	public int max_strikes;
	private int currentStrikes = 0;

	private const int MAX_STAGE = 2;

	private float driftMultiplier = 0;
	public GameObject carObject;
	public GameObject redCarObstacle;
	public EndlessRoadGeneration roadGener;
	public GameObject camera;

	private DriverControl carDriverControl;

	// Use this for initialization
	void Start () {
		score = 0.0f;
		carDriverControl = carObject.GetComponent<DriverControl> ();
		InvokeRepeating ("increaseTurnDelay", 10.0f, 10.0f);
		InvokeRepeating ("setDrift", 10.0f, 10.0f);
		InvokeRepeating ("increaseAccelerationDelay", 10.0f, 10.0f);
		InvokeRepeating ("changeLevel", 20.0f, 20.0f);
	}

	void FixedUpdate () {
		if (roadGener.currentViewerChunk.x == 0) {
			score += (carDriverControl.getCarSpeed ()/10000) * scoreMultiplier;
		}

	}
	void Update()
	{
		zoomCamera (0.1f);
		if (currentStrikes >= max_strikes) {
			endGame ();
		}
	}

	public void addStrike()
	{
		currentStrikes++;
	}

	void zoomCamera(float zoomSpeed)
	{
		camera.GetComponent<Camera> ().orthographicSize -= zoomSpeed * Time.deltaTime;
		if (camera.GetComponent<CameraFollow> ().cameraOffsetX != 0) {
			camera.GetComponent<CameraFollow> ().cameraOffsetX -= zoomSpeed * Time.deltaTime;
		}
		if (camera.GetComponent<CameraFollow> ().cameraOffsetY != 0) {
			camera.GetComponent<CameraFollow> ().cameraOffsetY -= zoomSpeed * Time.deltaTime;
		}
	}

	void setDrift()
	{
		driftMultiplier += 0.05f;
		carDriverControl.turnBias = (Random.value - 0.5f) * driftMultiplier;
	}

	void increaseTurnDelay()
	{
		carDriverControl.turnDelayAmount++;
	}

	void increaseAccelerationDelay()
	{
		carDriverControl.accelerationDelayAmount++;
	}

	void changeLevel()
	{
		if (levelUp) {
			stage++;
		} else {
			stage--;
		}
		if (stage == MAX_STAGE) {
			levelUp = false;
		} else {
			levelUp = true;
		}
		if (stage != 0) {
			levelUp = false;
		} else {
			levelUp = true;
		}
	}

	void increaseRedCarSpeed(float amount)
	{
		redCarObstacle.GetComponent<CarObstacle> ().carSpeed += amount;
	}

	public void endGame()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public int getStrikes()
	{
		return currentStrikes;
	}
}
