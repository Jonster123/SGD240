using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int worldSize = 16; // Number of chunks in each dimension
    public int chunkSize = 16; // Number of blocks in each chunk

    public Biome[] biomes; // Array of biome assets for different biomes

    private void Start()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                GenerateChunk(x * chunkSize, z * chunkSize);
            }
        }
    }

    private void GenerateChunk(int startX, int startZ)
    {
        int randomBiomeIndex = Random.Range(0, biomes.Length);
        Biome selectedBiome = biomes[randomBiomeIndex];

        for (int x = startX; x < startX + chunkSize; x++)
        {
            for (int z = startZ; z < startZ + chunkSize; z++)
            {
                int terrainHeight = Mathf.FloorToInt(Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 10) + 1;

                for (int y = 0; y < terrainHeight; y++)
                {
                    // Instantiate the selected biome prefab with weighted probability
                    GameObject blockPrefab = PickRandomPlant(selectedBiome);
                    Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);
                }
            }
        }
    }

    private GameObject PickRandomPlant(Biome biome)
    {
        // Calculate the total probability
        float totalProbability = 0f;
        foreach (float probability in biome.plantProbabilities)
        {
            totalProbability += probability;
        }

        // Choose a random value between 0 and the total probability
        float randomValue = Random.Range(0f, totalProbability);

        // Find the index of the chosen plant
        int chosenPlantIndex = -1;
        float cumulativeProbability = 0f;
        for (int i = 0; i < biome.plantProbabilities.Length; i++)
        {
            cumulativeProbability += biome.plantProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                chosenPlantIndex = i;
                break;
            }
        }

        // Return the chosen plant prefab
        return biome.plantPrefabs[chosenPlantIndex];
    }
}