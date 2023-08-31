using UnityEngine;

public class TerrainGenerator1 : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float peakHeight = 50f;
    public float voxelSize = 1f;

    private void Start()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);
    }

    private TerrainData GenerateTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, peakHeight, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    private float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        Vector2 offset = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + offset.x;
                float yCoord = (float)y / height * scale + offset.y;
                float heightValue = Mathf.PerlinNoise(xCoord, yCoord);
                heights[x, y] = heightValue * peakHeight;
            }
        }

        return heights;
    }
}