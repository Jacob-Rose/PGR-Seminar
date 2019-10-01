using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Obstacle : ScriptableObject {
	public GameObject obstacleTemplate;

	public int spawnChance;
	public bool shadowCast;
	public bool fadeAnim;
	public ChunkType[] chunkTypes;
	public int[] stages;
	public Vector2 xSpawnRange;
	public Vector2 ySpawnRange;
}
public enum ChunkType{ROAD,SHOLDER,BOUNDS};
