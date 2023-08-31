using System.Collections.Generic;
using UnityEngine;

public class TerrainAndBiomeGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int width = 100;
    public int depth = 100;
    public float scale = 20;
    public float moistureScale = 30;

    [Header("Biome Blocks")]
    public GameObject mountainBlock;
    public GameObject forestBlock;
    public GameObject desertBlock;
    public GameObject grasslandBlock;
    public GameObject beachBlock;
    public GameObject oceanBlock;
    public GameObject snowBlock;

    [Header("Biome Mesh Prefabs")]
    public List<GameObject> forestMeshes;
    public List<GameObject> desertMeshes;
    public List<GameObject> mountainMeshes;
    [Range(0, 1)]
    public float forestMeshDensity = 0.1f;
    [Range(0, 1)]
    public float desertMeshDensity = 0.05f;
    [Range(0, 1)]
    public float mountainMeshDensity = 0.03f;

    private void Start()
    {
        GenerateTerrainAndBiomes();
    }

    void GenerateTerrainAndBiomes()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float altitude = Mathf.PerlinNoise(x / scale, z / scale);
                float moisture = Mathf.PerlinNoise((x + width) / moistureScale, (z + depth) / moistureScale);

                GameObject toInstantiate = DetermineBlockBasedOnBiomeAndAltitude(altitude, moisture);
                Instantiate(toInstantiate, new Vector3(x, altitude * scale, z), Quaternion.identity);

                PlaceMeshBasedOnBiomeAndAltitude(new Vector3(x, altitude * scale + 1, z), altitude, moisture); //+1 in Y so mesh is above the block
            }
        }
    }

    GameObject DetermineBlockBasedOnBiomeAndAltitude(float altitude, float moisture)
    {
        if (altitude > 0.8f)
        {
            return snowBlock;
        }
        else if (altitude > 0.6f)
        {
            if (moisture < 0.33f)
            {
                return desertBlock;
            }
            else if (moisture < 0.66f)
            {
                return grasslandBlock;
            }
            else
            {
                return forestBlock;
            }
        }
        else if (altitude > 0.3f)
        {
            return beachBlock;
        }
        else
        {
            return oceanBlock;
        }
    }

    void PlaceMeshBasedOnBiomeAndAltitude(Vector3 position, float altitude, float moisture)
    {
        if (altitude > 0.8f && Random.value < mountainMeshDensity)
        {
            Instantiate(mountainMeshes[Random.Range(0, mountainMeshes.Count)], position, Quaternion.identity);
        }
        else if (altitude > 0.6f)
        {
            if (moisture < 0.33f && Random.value < desertMeshDensity)
            {
                Instantiate(desertMeshes[Random.Range(0, desertMeshes.Count)], position, Quaternion.identity);
            }
            else if (moisture > 0.66f && Random.value < forestMeshDensity)
            {
                Instantiate(forestMeshes[Random.Range(0, forestMeshes.Count)], position, Quaternion.identity);
            }
        }
    }
}