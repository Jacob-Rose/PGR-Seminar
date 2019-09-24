using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    //recieve data from Camera tracking system about which player is the lead player
    //set that player to be viewer transform
    // ^ this must updated

    public Transform viewerTransform;

    float stage = 1;

    //existing template for holding various sprites
    public int scenerySpriteCount;
    public Sprite[] sholderSprites;
    public int sholderSpriteCount;
    public Sprite[] roadSprites;
    public int roadSpriteCount;

    public GameObject roadTemplate;
    public GameObject boundsTemplate;

    //I am assuming this is used to regulate sprite size
    public float chunkSize
    {
        get
        {
            return (((roadTemplate.GetComponent<SpriteRenderer>().sprite.texture.height) / 100) * roadTemplate.transform.localScale.y);
        }
    }

    //this finds what chunk to spawn the next road sprite based on the viewer position
    public Vector2 currentViewerChunk
    {
        get
        {
            return new Vector2(Mathf.RoundToInt(viewerTransform.position.x / chunkSize), Mathf.RoundToInt(viewerTransform.position.y / chunkSize));
        }
    }
    //this is probably to say how far away from the viewer to spawn the first chunk
    public int chunkViewDistance;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //viewerTransform = Camera.playerlead;
        UpdateChunks();
    }

    //this is what populates the chunks with things like obstacles and pretty sprites
    // I am assuming that eventTracker.stage is used to see how many obstacles to input based on the level the user is playing in
    GameObject createChunk(int x, int y)
    {
        /*
        GameObject tile;
        if (x == 0)
        {
            tile = Instantiate(roadTemplate, this.transform);
            tile.name = "Tile " + x + " " + y;
            tile.transform.position = new Vector3((x * chunkSize), (y * chunkSize), 0);

            int spriteIndex = Random.Range(0, roadSpriteCount);

            tile.GetComponent<SpriteRenderer>().sprite = roadSprites[(stage * roadSpriteCount) + spriteIndex];

        }
        else if (x == -1 || x == 1)
        {
            tile = Instantiate(roadTemplate, this.transform);
            tile.name = "Tile " + x + " " + y;
            tile.transform.position = new Vector3((x * chunkSize), (y * chunkSize), 0);
            //GenerateObstacles (new Vector2 (x, y));

            int spriteIndex = Random.Range(0, sholderSpriteCount);

            tile.GetComponent<SpriteRenderer>().sprite = sholderSprites[(stage * sholderSpriteCount) + spriteIndex];


        }
        else
        {
            tile = Instantiate(boundsTemplate, this.transform);
            tile.name = "Tile " + x + " " + y;
            tile.transform.position = new Vector3((x * chunkSize), (y * chunkSize), 0);

            //int spriteIndex = Random.Range (0, scenerySpriteCount);

            int spriteIndex = 0;
            tile.GetComponent<SpriteRenderer>().sprite = scenerySprites[(stage * scenerySpriteCount) + spriteIndex];
        }
        return tile;
        */
        return new GameObject();
    }

    //this is what spawns the next chunk containing all information into the next spot
    Dictionary<Vector2, GameObject> terrainChunkDictionary = new Dictionary<Vector2, GameObject>();
    private void UpdateChunks()
    {

        for (int i = -chunkViewDistance; i < chunkViewDistance; i++)
        {
            for (int k = -chunkViewDistance; k < chunkViewDistance; k++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentViewerChunk.x + i, currentViewerChunk.y + k);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, createChunk((int)viewedChunkCoord.x, (int)viewedChunkCoord.y));

                }
            }
        }
    }

    /*
     * What info needs to be sent to the server:
     * Option 1:
     * Player scripts determine most things, all the server does is send the correct position to generate prefabs and obstacles
     * 
     * Option 2:
     * Player script only sends position necessary, server decides tile to generate, prefab data, obstacle data, and player recieves this to spawn in the tile
     * 
     * Afterwards:
     * Player sends that it recieved the info
     * Server acknowledges this
     * 
     * Finally:
     * Tiles get destroyed after they fall out of camera sight
     * 
     * 
     */




}
