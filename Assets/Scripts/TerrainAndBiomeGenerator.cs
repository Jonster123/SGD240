using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    public string name;
    public GameObject blockPrefab;
    public List<GameObject> meshPrefabs;
    [Range(0, 1)]
    public float meshDensity;
    public float minHeight;
    public float maxHeight;
    public float minMoisture;
    public float maxMoisture;
}

public class TerrainAndBiomeGenerator : MonoBehaviour
{
    [Header("General Settings")]
    public int seed = 0;

    [Header("Terrain Settings")]
    public int width = 100;
    public int depth = 100;
    public float scale = 20;
    public float moistureScale = 30;

    [Header("Biome Settings")]
    public List<Biome> biomes;

    private void Start()
    {
        Random.InitState(seed); // Initialize random number generator with the seed
        GenerateTerrainAndBiomes();
    }

    void GenerateTerrainAndBiomes()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float altitude = Mathf.PerlinNoise(seed + x / scale, seed + z / scale);
                float moisture = Mathf.PerlinNoise(seed + (x + width) / moistureScale, seed + (z + depth) / moistureScale);

                Biome biome = DetermineBiome(altitude, moisture);

                if (biome != null)
                {
                    Instantiate(biome.blockPrefab, new Vector3(x, altitude * scale, z), Quaternion.identity);
                    PlaceMesh(biome, new Vector3(x, altitude * scale + 1, z));
                }
            }
        }
    }

    Biome DetermineBiome(float altitude, float moisture)
    {
        foreach (Biome biome in biomes)
        {
            if (altitude >= biome.minHeight && altitude <= biome.maxHeight &&
                moisture >= biome.minMoisture && moisture <= biome.maxMoisture)
            {
                return biome;
            }
        }
        return null;
    }

    void PlaceMesh(Biome biome, Vector3 position)
    {
        if (Random.value < biome.meshDensity)
        {
            Instantiate(biome.meshPrefabs[Random.Range(0, biome.meshPrefabs.Count)], position, Quaternion.identity);
        }
    }
}