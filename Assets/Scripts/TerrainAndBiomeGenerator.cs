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
    public int chunkSize = 16;
    public int renderDistance = 2;
    public int maxChunks = 20;  // Maximum number of chunks in one direction

    [Header("Terrain Settings")]
    public float scale = 20;
    public float moistureScale = 30;

    [Header("Biome Settings")]
    public List<Biome> biomes;

    private Transform playerTransform;
    private Vector3 lastPlayerPosition;
    private Dictionary<Vector2, GameObject> activeChunks;

    private void Start()
    {
        Random.InitState(seed);
        playerTransform = Camera.main.transform;
        activeChunks = new Dictionary<Vector2, GameObject>();
        lastPlayerPosition = playerTransform.position;
        GenerateChunksAroundPlayer();
    }

    private void Update()
    {
        Vector3 playerPosition = playerTransform.position;
        if (Vector3.Distance(lastPlayerPosition, playerPosition) > chunkSize)
        {
            lastPlayerPosition = playerPosition;
            GenerateChunksAroundPlayer();
        }
    }

    private void GenerateChunksAroundPlayer()
    {
        List<Vector2> chunksToRemove = new List<Vector2>();

        foreach (var chunkPosition in activeChunks.Keys)
        {
            if (Vector2.Distance(new Vector2(playerTransform.position.x, playerTransform.position.z), chunkPosition) > chunkSize * (renderDistance + 1))
            {
                chunksToRemove.Add(chunkPosition);
            }
        }

        foreach (var chunkPosition in chunksToRemove)
        {
            Destroy(activeChunks[chunkPosition]);
            activeChunks.Remove(chunkPosition);
        }

        int playerChunkX = Mathf.FloorToInt(playerTransform.position.x / chunkSize);
        int playerChunkZ = Mathf.FloorToInt(playerTransform.position.z / chunkSize);

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2 chunkPosition = new Vector2((playerChunkX + x) * chunkSize, (playerChunkZ + z) * chunkSize);
                if (!activeChunks.ContainsKey(chunkPosition))
                {
                    GenerateChunk(chunkPosition);
                }
            }
        }
    }

    private void GenerateChunk(Vector2 chunkPosition)
    {
        if (Mathf.FloorToInt(chunkPosition.x / chunkSize) >= maxChunks || Mathf.FloorToInt(chunkPosition.y / chunkSize) >= maxChunks)
        {
            return;
        }

        GameObject newChunk = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.y}");
        newChunk.transform.position = new Vector3(chunkPosition.x, 0, chunkPosition.y);
        activeChunks.Add(chunkPosition, newChunk);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float altitude = Mathf.PerlinNoise(seed + (chunkPosition.x + x) / scale, seed + (chunkPosition.y + z) / scale);
                float moisture = Mathf.PerlinNoise(seed + (chunkPosition.x + x + chunkSize) / moistureScale, seed + (chunkPosition.y + z + chunkSize) / moistureScale);

                Biome biome = DetermineBiome(altitude, moisture);

                if (biome != null)
                {
                    Instantiate(biome.blockPrefab, new Vector3(chunkPosition.x + x, altitude * scale, chunkPosition.y + z), Quaternion.identity, newChunk.transform);
                    PlaceMesh(biome, new Vector3(chunkPosition.x + x, altitude * scale + 1, chunkPosition.y + z), newChunk.transform);
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

    void PlaceMesh(Biome biome, Vector3 position, Transform parentTransform)
    {
        // Use position-based seed to ensure deterministic randomness
        int deterministicSeed = seed ^ (int)position.x ^ ((int)position.z << 16) ^ ((int)position.y << 8);
        Random.InitState(deterministicSeed);

        if (Random.value < biome.meshDensity)
        {
            GameObject prefab = biome.meshPrefabs[Random.Range(0, biome.meshPrefabs.Count)];
            Instantiate(prefab, position, Quaternion.identity, parentTransform);
        }

        // Reset random state to global state to ensure other random behaviors in your game aren't affected.
        Random.InitState(System.Environment.TickCount);
    }
}
