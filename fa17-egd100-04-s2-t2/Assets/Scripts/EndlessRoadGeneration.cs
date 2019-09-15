using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoadGeneration : MonoBehaviour {

    public Transform viewerTransform;
	public EventTracker eventTracker;

	public Sprite[] scenerySprites; //each array in the array holds sprites for certain stage
	public int scenerySpriteCount;
	public Sprite[] sholderSprites;
	public int sholderSpriteCount;
	public Sprite[] roadSprites;
	public int roadSpriteCount; 


	public GameObject roadTemplate;
	public GameObject boundsTemplate;



	public float chunkSize {
		get {
			return (((roadTemplate.GetComponent<SpriteRenderer> ().sprite.texture.height) / 100) * roadTemplate.transform.localScale.y);
		}
	}

	public Vector2 currentViewerChunk
	{
		get {
			return new Vector2 (Mathf.RoundToInt (viewerTransform.position.x / chunkSize), Mathf.RoundToInt (viewerTransform.position.y / chunkSize));
		}
	}
	public int chunkViewDistance;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		UpdateChunks ();
	}

	GameObject createChunk(int x, int y)
	{
		GameObject tile;
		if (x == 0) {
			tile = Instantiate (roadTemplate, this.transform);
			tile.name = "Tile " + x + " " + y;
			tile.transform.position = new Vector3 ((x * chunkSize), (y * chunkSize), 0);

			int spriteIndex = Random.Range (0, roadSpriteCount);

			tile.GetComponent<SpriteRenderer> ().sprite = roadSprites [(eventTracker.stage * roadSpriteCount) + spriteIndex];

		} else if (x == -1 || x == 1) {
			tile = Instantiate (roadTemplate, this.transform);
			tile.name = "Tile " + x + " " + y;
			tile.transform.position = new Vector3 ((x * chunkSize), (y * chunkSize), 0);
			//GenerateObstacles (new Vector2 (x, y));

			int spriteIndex = Random.Range (0, sholderSpriteCount);

			tile.GetComponent<SpriteRenderer> ().sprite = sholderSprites [(eventTracker.stage * sholderSpriteCount) + spriteIndex];


		} else {
			tile = Instantiate (boundsTemplate, this.transform);
			tile.name = "Tile " + x + " " + y;
			tile.transform.position = new Vector3 ((x * chunkSize), (y * chunkSize), 0);

			//int spriteIndex = Random.Range (0, scenerySpriteCount);

			int spriteIndex = 0;
			tile.GetComponent<SpriteRenderer> ().sprite = scenerySprites [(eventTracker.stage * scenerySpriteCount) + spriteIndex];
		}
		return tile;
	}

	Dictionary<Vector2, GameObject> terrainChunkDictionary = new Dictionary<Vector2, GameObject>();
	private void UpdateChunks() {
		
		for (int i = -chunkViewDistance; i < chunkViewDistance; i++) {
			for (int k = -chunkViewDistance; k < chunkViewDistance; k++) {
				Vector2 viewedChunkCoord = new Vector2 (currentViewerChunk.x + i, currentViewerChunk.y + k);

				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
				} else {
					terrainChunkDictionary.Add(viewedChunkCoord, createChunk((int)viewedChunkCoord.x,(int)viewedChunkCoord.y));

				}
			}
		}
	}

}