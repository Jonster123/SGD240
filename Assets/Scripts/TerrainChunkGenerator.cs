using System.Collections.Generic;
using UnityEngine;

public class TerrainChunkGenerator : MonoBehaviour
{
    public GameObject blockPrefab;
    public int chunkSize = 16; // Size of each chunk
    public int renderDistance = 2; // Number of chunks to render around the player

    private Transform playerTransform; // Reference to the player's Transform
    private Vector3 lastPlayerPosition; // Last position of the player
    private Dictionary<Vector2, GameObject> activeChunks; // Dictionary to store active chunks

    void Start()
    {
        playerTransform = Camera.main.transform; // Assume the player's position is at the camera for this example
        lastPlayerPosition = playerTransform.position;
        activeChunks = new Dictionary<Vector2, GameObject>();

        GenerateInitialChunks();
    }

    void Update()
    {
        Vector3 playerPosition = playerTransform.position;
        if (Vector3.Distance(lastPlayerPosition, playerPosition) > chunkSize)
        {
            lastPlayerPosition = playerPosition;
            GenerateChunksAroundPlayer();
        }
    }

    void GenerateInitialChunks()
    {
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                GenerateChunk(new Vector2(x * chunkSize, z * chunkSize));
            }
        }
    }

    void GenerateChunksAroundPlayer()
    {
        int playerChunkX = Mathf.FloorToInt(playerTransform.position.x / chunkSize);
        int playerChunkZ = Mathf.FloorToInt(playerTransform.position.z / chunkSize);

        // Remove chunks that are out of the render distance
        List<Vector2> chunksToRemove = new List<Vector2>();
        foreach (var chunkPosition in activeChunks.Keys)
        {
            int chunkX = Mathf.FloorToInt(chunkPosition.x / chunkSize);
            int chunkZ = Mathf.FloorToInt(chunkPosition.y / chunkSize);
            if (Mathf.Abs(chunkX - playerChunkX) > renderDistance || Mathf.Abs(chunkZ - playerChunkZ) > renderDistance)
            {
                chunksToRemove.Add(chunkPosition);
            }
        }

        foreach (var chunkPosition in chunksToRemove)
        {
            Destroy(activeChunks[chunkPosition]);
            activeChunks.Remove(chunkPosition);
        }

        // Generate new chunks around the player
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

    void GenerateChunk(Vector2 chunkPosition)
    {
        GameObject newChunk = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.y}");
        newChunk.transform.position = new Vector3(chunkPosition.x, 0, chunkPosition.y);
        activeChunks.Add(chunkPosition, newChunk);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                Instantiate(blockPrefab, new Vector3(chunkPosition.x + x, 0, chunkPosition.y + z), Quaternion.identity, newChunk.transform);
            }
        }
    }
}