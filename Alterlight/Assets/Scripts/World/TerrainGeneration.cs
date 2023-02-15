using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using static ExtensionMethods.ExtensionMethods;

public class TerrainGeneration : MonoBehaviour
{
    #region Attributes

    [Header("Tile Sprites")]
    public Sprite sand;
    public Sprite sandFoliage;
    public Sprite grass;
    public Sprite grassFoliage;
    public Sprite tree;
    public Sprite snow;
    public Sprite snowTree;
    public Sprite water;
    public Sprite closed;
    public Sprite open;
    public Sprite sidewaysClosed;
    public Sprite sidewaysOpen;
    public Sprite torchParticleSprite;
    public Material defaultMaterial;

    [Header("Mobs")]
    public GameObject mobParent;
    public GameObject beetle;
    [Range(0.0f, 1.0f)]
    public float beetleSpawnRate;
    private int beetleCount = 0;


    [Header("Structures")]
    public GameObject desertTemple;

    [Header("Frequency Settings")]
    [Range(0.0f, 1.0f)]
    public float noiseFreq = 0.03f;
    [Range(0.0f, 1.0f)]
    public float treeChance;
    [Range(0.0f, 1.0f)]
    public float snowTreeChance;
    [Range(0.0f, 1.0f)]
    public float grassFoliageFreq;
    [Range(0.0f, 1.0f)]
    public float sandFoliageFreq;
    [Range(0.0f, 1.0f)]
    public float desertTempleFreq;

    [Header("World Settings")]
    public int worldWidth = 200;
    public int worldHeight = 200;
    public int chunkSize = 16;
    public float seed;
    public Texture2D noiseTexture;

    [Header("Misc")]
    public GameObject[] worldChunks;
    public Vector2 treeColOffset = new Vector2(-0.03381205f, -0.1280592f);
    public Vector2 treeColSize = new Vector2(0.45925f, 0.480186f);
    public GameObject tileDrop;
    public List<Vector2> worldTiles = new List<Vector2>();
    public List<GameObject> worldTileObjects = new List<GameObject>();
    public PlayerMovement player;
    public DayCycle dayCycle;

    [Header("Private fields")]
    private List<string> mainTiles = new List<string>{ "grass", "sand", "water", "snow", "grassfoliage", "sandfoliage" }; //? World tiles (unbreakable)
    private List<string> unplaceableTiles = new List<string>{ "water", "tree", "snowtree" }; //? Player can't build over these (world tiles, use List<string>.Contains(string))
    private List<string> buildingBlocksCol = new List<string>{ "stoneblock", "woodblock", "door" }; //? Player can't build over these (require collider, use ContainsAny(string, List<string>))
    private List<string> buildingBlocksAll = new List<string>{ "stoneblock", "woodblock", "door", "torch" }; //? Player can't build over these (all. ContainsAny(string, List<string>))
    public GameObject oldTile;
    private List<Item> placedItems = new List<Item>();
    private List<Vector2> itemPosition = new List<Vector2>();
    private int doorCount = 0;

    #endregion

    private void Start()
    {
        seed = UnityEngine.Random.Range(-10000, 10000);

        DateTime before = DateTime.Now;

        GenerateNoiseTexture();
        CreateChunks();
        GenerateTerrain();

        DateTime after = DateTime.Now; 

        TimeSpan duration = after.Subtract(before);
        Print($"Generation time: {duration.Milliseconds} ms");
    }
    public void CreateChunks()
    {
        int numChunks = worldHeight / chunkSize;
        worldChunks = new GameObject[numChunks];

        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
    public void GenerateTerrain()
    {
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float currColor = noiseTexture.GetPixel(x, y).r;

                #region Water
                if (currColor < 0.2f)
                {
                    CreateTile("water", water, x, y);
                }
                #endregion

                #region Sand
                else if (currColor >= 0.2f && currColor <= 0.35f)
                {
                    System.Random r = new System.Random();
                    if (r.NextDouble() < sandFoliageFreq)
                        CreateTile("sandfoliage", sandFoliage, x, y);
                    else
                        CreateTile("sand", sand, x, y);

                    if (r.NextDouble() < desertTempleFreq)
                        Instantiate(desertTemple, new Vector2(x, y), Quaternion.identity);
                    
                    if (r.NextDouble() < beetleSpawnRate)
                    {
                        beetleCount++;
                        GameObject b = Instantiate(beetle, new Vector2(x, y), Quaternion.identity);
                        b.name = $"beetle {beetleCount}";
                        b.AddComponent<Beetle>();
                        b.transform.parent = mobParent.transform;
                    }
                }
                #endregion

                #region Grass
                else if (currColor > 0.35f && currColor <= 0.82f)
                {
                    System.Random r = new System.Random();
                    if (r.NextDouble() < grassFoliageFreq)
                        CreateTile("grassfoliage", grassFoliage, x, y);
                    else
                    {
                        CreateTile("grass", grass, x, y);
                        if (r.NextDouble() < treeChance)
                            CreateTile("tree", tree, x, y);
                    }
                }
                #endregion

                #region Snow
                else 
                {
                    System.Random r = new System.Random();
                    CreateTile("snow", snow, x, y);
                        if (r.NextDouble() < snowTreeChance)
                            CreateTile("snowtree", snowTree, x, y);
                }
                #endregion
            }
        }
    }
    public void CreateTile(string n, Sprite sprite, int x, int y) 
    {
        GameObject newTile = new GameObject();
        newTile.name = n;

        float chunkCoord = (Mathf.Round(x / chunkSize) * chunkSize);
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = sprite;
        if (n == "tree" || n == "snow tree")
        {
            newTile.GetComponent<SpriteRenderer>().sortingOrder = 3;
            newTile.AddComponent<BoxCollider2D>();
            newTile.GetComponent<BoxCollider2D>().offset = treeColOffset;
            newTile.GetComponent<BoxCollider2D>().size = treeColSize;
        }
        if (n == "water")
        {
            newTile.AddComponent<WaterProp>();
        }

        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
        worldTileObjects.Add(newTile);
    }
    public bool PlayerPlaceTile(Item item, string n, Sprite spr, int x, int y)
    {
        bool isAllBuildingBlockAlready = ContainsAny(GetTile(x, y).name, buildingBlocksAll);
        
        if (isAllBuildingBlockAlready || unplaceableTiles.Contains(GetTile(x, y).name))
            return false;
        else
        {
            #region Setup
            GameObject newTile = new GameObject();

            if (n == "door")
            {
                doorCount++;
                newTile.name = n + " " + doorCount;
            }
            else    
                newTile.name = n;

            float chunkCoord = (Mathf.Round(x / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;

            newTile.transform.parent = worldChunks[(int)chunkCoord].transform;
            newTile.AddComponent<SpriteRenderer>();
            newTile.GetComponent<SpriteRenderer>().sprite = spr;
            newTile.GetComponent<SpriteRenderer>().sortingOrder = 2;
            #endregion

            #region Special properties if needed

            // If block is a torch or something else, dont set collider
            if (buildingBlocksCol.Contains(n))
                newTile.AddComponent<BoxCollider2D>();

            if (newTile.name == "torch")
            {
                newTile.AddComponent<Light2D>();
                newTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
                Torch t = newTile.AddComponent<Torch>();
                t.dayCycle = dayCycle;
                t.torchParticle = torchParticleSprite;
                t.material = defaultMaterial;
            }

            if (n == "door")
            {
                newTile.AddComponent<Door>();
                if (player.lastDirFaced.x is 1 or -1 && player.lastDirFaced.y == 0) 
                {
                    newTile.GetComponent<Door>().facingSide = true;
                    newTile.GetComponent<SpriteRenderer>().sprite = sidewaysClosed;
                    newTile.GetComponent<Door>().sidewaysClosed = sidewaysClosed;
                    newTile.GetComponent<Door>().sidewaysOpen = sidewaysOpen;
                }
                else
                {
                    newTile.GetComponent<Door>().facingSide = false;
                    newTile.GetComponent<Door>().closed = closed;
                    newTile.GetComponent<Door>().open = open;
                }
            }
            #endregion

            #region Place and add new tile and item to list
            oldTile = GetTile(x, y);
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
            newTile.transform.SetAsFirstSibling();

            // Add the new tile to the arrays
            Debug.Log(worldTileObjects.IndexOf(oldTile));

            int index = worldTiles.IndexOf(new Vector2(newTile.transform.position.x - 0.5f, newTile.transform.position.y - 0.5f)); 

            worldTiles[index] = newTile.transform.position - (Vector3.one * 0.5f);
            worldTileObjects[index] = newTile;


            // Render a tile under the placed tile so when broken there is something underneath
            oldTile = GameObject.Instantiate(oldTile);
            oldTile.transform.parent = worldChunks[(int)chunkCoord].transform;

            // Add placed item and its position to array (used in GetTile(int, int))
            placedItems.Add(item);
            itemPosition.Add(new Vector2(x, y));
            #endregion

            return true;
        }
    }
    public void BreakTile(int x, int y)
    {
        Item item = null;

        if (worldTiles.Contains(new Vector2Int(x, y)) && x >= 0 && x <= worldWidth && y >= 0 && y <= worldHeight)
        {
            GameObject tile = worldTileObjects[worldTiles.IndexOf(new Vector2(x, y))];
            bool isNotWorldTile = !ContainsAny(tile.name, mainTiles);

            if (isNotWorldTile)
            {
                int index = worldTiles.IndexOf(new Vector2(tile.transform.position.x - 0.5f, tile.transform.position.y - 0.5f));

                worldTiles[index] = tile.transform.position - (Vector3.one * 0.5f);
                worldTileObjects[index] = oldTile;

                Destroy(tile);  

                // Get the item
                for (int i = 0; i < itemPosition.Count; i++)
                {
                    if (x == itemPosition[i].x && y == itemPosition[i].y)
                        item = placedItems[i];
                }

                GameObject newTileDrop = Instantiate(tileDrop, new Vector2(x, y + 2), Quaternion.identity);
                newTileDrop.GetComponent<SpriteRenderer>().sprite = tile.GetComponent<SpriteRenderer>().sprite;
                newTileDrop.GetComponent<TileDropController>().item = item;

                StartCoroutine(StopTileDrop(newTileDrop, 0.2f));
            }
        }
    }
    private bool ContainsAny(string tileName, List<string> list)
    {
        List<bool> contains = new List<bool>();

        foreach (string s in list)
            contains.Add(tileName.Contains(s));

        return contains.Contains(true);
    }
    private IEnumerator StopTileDrop(GameObject tile, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tile != null)
            tile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
    public GameObject GetTile(int x, int y)
    {
        if (worldTiles.Contains(new Vector2Int(x, y)) && x >= 0 && x <= worldWidth && y >= 0 && y <= worldHeight)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < worldTiles.Count; i++)
            {
                if (worldTiles[i] == new Vector2(x, y))
                    indices.Add(i);
            }

            List<GameObject> allObjectsAtPos = new List<GameObject>();
            foreach (int index in indices)
                allObjectsAtPos.Add(worldTileObjects[index]);
            
            foreach (GameObject g in allObjectsAtPos)
            {
                if (g.name == "tree" || g.name == "snowtree")
                    return g;
            }

            GameObject tile = worldTileObjects[worldTiles.IndexOf(new Vector2(x, y))];
            return tile;  
        }
        return null;
    }
    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldWidth, worldHeight);
        
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * noiseFreq, (y + seed) * noiseFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }
}