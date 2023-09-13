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

    // To keep track of instantiated blocks
    private GameObject[,] blockMap;

    private void Start()
    {
        Random.InitState(seed);
        blockMap = new GameObject[width, depth]; // Initialize blockMap
        GenerateTerrainAndBiomes();
    }

    void GenerateTerrainAndBiomes()
    {
        Biome[,] biomeMap = new Biome[width, depth];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float altitude = Mathf.PerlinNoise(seed + x / scale, seed + z / scale);
                float moisture = Mathf.PerlinNoise(seed + (x + width) / moistureScale, seed + (z + depth) / moistureScale);

                Biome biome = DetermineBiome(altitude, moisture);
                biomeMap[x, z] = biome;

                if (biome != null)
                {
                    GameObject newBlock = Instantiate(biome.blockPrefab, new Vector3(x, altitude * scale, z), Quaternion.identity);
                    blockMap[x, z] = newBlock; // Store the new block
                    PlaceMesh(biome, new Vector3(x, altitude * scale + 1, z));
                }
            }
        }

        for (int z = 1; z < depth - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (biomeMap[x, z]?.name == "Ocean")
                {
                    ReplaceAdjacentWithBeach(x, z, biomeMap);
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

    void ReplaceAdjacentWithBeach(int x, int z, Biome[,] biomeMap)
    {
        Biome beachBiome = biomes.Find(b => b.name == "Beach");
        if (beachBiome == null) return;

        int range = 10; // Change this value to adjust the range. E.g., 2 for a 5x5 grid, 1 for a 3x3 grid.

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dz = -range; dz <= range; dz++)
            {
                int newX = x + dx;
                int newZ = z + dz;

                // Check bounds
                if (newX >= 0 && newX < width && newZ >= 0 && newZ < depth)
                {
                    if (biomeMap[newX, newZ]?.name != "Ocean")
                    {
                        GameObject oldBlock = blockMap[newX, newZ];
                        if (oldBlock != null)
                        {
                            Destroy(oldBlock); // Remove the existing block
                        }

                        float altitude = Mathf.PerlinNoise(seed + newX / scale, seed + newZ / scale);
                        GameObject newBlock = Instantiate(beachBiome.blockPrefab, new Vector3(newX, altitude * scale, newZ), Quaternion.identity);
                        blockMap[newX, newZ] = newBlock; // Store the new block
                        biomeMap[newX, newZ] = beachBiome; // Update the biomeMap

                        PlaceMesh(beachBiome, new Vector3(newX, altitude * scale + 1, newZ));
                    }
                }
            }
        }
    }

}
