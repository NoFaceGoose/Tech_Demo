using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelGenerator : MonoBehaviour
{
    [Tooltip("The Tilemap to draw onto")]
    public Tilemap tilemap;
    [Tooltip("The Tile to draw (use a RuleTile for best results)")]
    public TileBase tile;

    [Tooltip("Width of our map")]
    public int width;
    [Tooltip("Height of our map")]
    public int height;

    [Tooltip("The settings of our map")]
    public MapSettings mapSetting;

    public GameObject spike;
    private GameObject[] spikes;
    public GameObject potion;
    private GameObject[] potions;
    public GameObject playerEvent;
    private GameObject[] playerEvents;
    public GameObject boss;
    public GameObject player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ClearMap();
            GenerateMap();
        }
    }

    [ExecuteInEditMode]
    public void GenerateMap()
    {
        ClearMap();

        float seed;
        if (mapSetting.randomSeed)
        {
            seed = Time.time;
        }
        else
        {
            seed = mapSetting.seed;
        }

        // Generate our array
        int[,] map = GenerateArray(width, height, true);
        // Generate the perlin noise onto the array
        map = PerlinNoiseCave(map, mapSetting.modifier, mapSetting.edgesAreWalls);

        // Render the result
        RenderMap(map, tilemap, tile, spike);
    }

    public void ClearMap()
    {
        tilemap.ClearAllTiles();

        if (spikes != null)
        {
            for (int i = 0; i < spikes.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(spikes[i]);
                }
                else
                {
                    DestroyImmediate(spikes[i]);
                }
            }
        }
        spikes = null;

        if (potions != null)
        {
            for (int i = 0; i < potions.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(potions[i]);
                }
                else
                {
                    DestroyImmediate(potions[i]);
                }
            }
        }
        potions = null;

        if (playerEvents != null)
        {
            for (int i = 0; i < playerEvents.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(playerEvents[i]);
                }
                else
                {
                    DestroyImmediate(playerEvents[i]);
                }
            }
        }
        playerEvents = null;
    }

    private int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }

    /// <summary>
    /// Creates a cave using perlin noise for the generation process
    /// </summary>
    /// <param name="map">the map to be modified</param>
    /// <param name="modifier">the value we times the position by to get our perlin gen</param>
    /// <param name="edgesAreWalls">If set to <c>true</c> edges are walls.</param>
    /// <returns>The noise cave.</returns>
    private int[,] PerlinNoiseCave(int[,] map, float modifier, bool edgesAreWalls)
    {
        int newPoint;
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {

                if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1) - 1))
                {
                    // Keep the edges as walls
                    map[x, y] = 1;
                }
                else
                {
                    // Generate a new point using perlin noise, then round it to a value of either 0 or 1
                    newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
                    map[x, y] = newPoint;
                }
            }
        }
        return map;
    }

    private void RenderMap(int[,] map, Tilemap tilemap, TileBase tile, GameObject spike)
    {
        tilemap.ClearAllTiles(); // Clear the map (avoid overlapping)

        for (int x = 0; x < map.GetUpperBound(0); x++) // Loop through the width of the map
        {
            for (int y = 0; y < map.GetUpperBound(1); y++) // Loop through the height of the map
            {
                if (map[x, y] == 1) // 0 = empty, 1 = tile, 2 = spike, 3 = potion, 4 = player event
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
                else if (map[x, y] == 0)
                {
                    // Set some blocks which are not tiles as spikes by random in a certain range
                    if (Random.Range(0, 100) < mapSetting.dangerDegree)
                    {
                        if (x > 0 & y > 0 && x < map.GetUpperBound(0) - 1 && y < map.GetUpperBound(1) - 1)
                        {
                            // Spike always attach to one title with a different angle
                            // And a spike will never be set between two tiles
                            if (map[x - 1, y] == 0 && map[x + 1, y] == 0)
                            {
                                if (map[x, y - 1] == 1 && map[x, y + 1] == 0)
                                {
                                    Instantiate(spike, tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0)), spike.transform.rotation);
                                    map[x, y] = 2;
                                    continue;
                                }

                                if (map[x, y + 1] == 1 && map[x, y - 1] == 0)
                                {
                                    Instantiate(spike, tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0)), Quaternion.Euler(0, 0, 180));
                                    map[x, y] = 2;
                                    continue;
                                }
                            }

                            if (map[x, y + 1] == 0 && map[x, y - 1] == 0)
                            {
                                if (map[x - 1, y] == 1 && map[x + 1, y] == 0)
                                {
                                    Instantiate(spike, tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0)), Quaternion.Euler(0, 0, -90));
                                    map[x, y] = 2;
                                    continue;
                                }
                                if (map[x + 1, y] == 1 && map[x - 1, y] == 0)
                                {
                                    Instantiate(spike, tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0)), Quaternion.Euler(0, 0, 90));
                                    map[x, y] = 2;
                                    continue;
                                }
                            }
                        }
                    }

                    // Generate potions, which can only be in one empty block and on the top of one tile
                    if (y > 0 && map[x, y - 1] == 1 && Random.Range(0, 100) < mapSetting.potionNum)
                    {
                        Instantiate(potion, tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0)), potion.transform.rotation);
                        map[x, y] = 3;
                        continue;
                    }

                    // Generate player events, which can only be in one empty block and 'embraced' by two tiles or spikes
                    if (Random.Range(0, 100) < mapSetting.playerEventNum && x > 0 && y > 0 && x < map.GetUpperBound(0) - 1 && y < map.GetUpperBound(1) - 1)
                    {
                        if (
                            ((map[x, y + 1] == 1 || map[x, y + 1] == 2) && (map[x - 1, y] == 1 || map[x - 1, y] == 2 || map[x + 1, y] == 1 || map[x + 1, y] == 2))
                            ||
                            ((map[x, y - 1] == 1 || map[x, y - 1] == 2) && (map[x - 1, y] == 1 || map[x - 1, y] == 2 || map[x + 1, y] == 1 || map[x + 1, y] == 2))
                            )
                        {
                            Instantiate(playerEvent, tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0)), playerEvent.transform.rotation);
                            map[x, y] = 4;
                        }
                    }
                }
            }
        }

        spikes = GameObject.FindGameObjectsWithTag("Spike");
        potions = GameObject.FindGameObjectsWithTag("Potion");
        playerEvents = GameObject.FindGameObjectsWithTag("Player Event");

        // Generate the player's initial position, which must be on a tile
        int generationLimit = 0;
        while (true)
        {
            int playerTile = Random.Range(0, map.GetUpperBound(0));
            if (map[playerTile, map.GetUpperBound(1) - 1] == 1)
            {
                player.transform.position = tilemap.GetCellCenterWorld(new Vector3Int(playerTile, map.GetUpperBound(1), 0));
                break;
            }

            // In case there is no tile in the top row
            generationLimit++;
            if (generationLimit > map.GetUpperBound(0))
            {
                player.transform.position = tilemap.GetCellCenterWorld(new Vector3Int(0, map.GetUpperBound(1), 0));
                break;
            }
        }

        // Generate the boss fight stage width and offset by random
        int maxStageWidth = map.GetUpperBound(0) / 2 > 10 ? map.GetUpperBound(0) / 2 : 10;
        int battleStageWidth = Random.Range(10, maxStageWidth);
        int offset = Random.Range(0, map.GetUpperBound(0) - battleStageWidth);

        // Set the boss fight stage
        for (int y = -6; y < 0; y++)
        {
            for (int x = offset; x < battleStageWidth + offset; x++)
            {
                if (y != -1 || x == offset || x == battleStageWidth + offset - 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        // Generate the boss's position on the stage by random
        int bossTile = Random.Range(offset + 1, battleStageWidth + offset - 1);
        Vector3 tilePosition = tilemap.GetCellCenterWorld(new Vector3Int(bossTile, -1, 0));
        boss.transform.position = new Vector3(tilePosition.x, tilePosition.y - 0.4f, tilePosition.z);
    }
}

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //Reference to our script
        LevelGenerator levelGen = (LevelGenerator)target;

        //Only show the mapsettings UI if we have a reference set up in the editor
        if (levelGen.mapSetting != null)
        {
            Editor mapSettingEditor = CreateEditor(levelGen.mapSetting);
            mapSettingEditor.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                levelGen.GenerateMap();
            }

            if (GUILayout.Button("Clear"))
            {
                levelGen.ClearMap();
            }
        }
    }
}