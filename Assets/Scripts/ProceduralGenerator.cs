using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    public int width = 100;
    public int depth = 100;
    public float scale = 20;

    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    public GameObject waterBlock;
    public GameObject sandBlock;
    public GameObject grassBlock;
    public GameObject snowBlock;

    private void Start()
    {
        GenerateTerrain();
    }

    float GetHeight(float x, float z)
    {
        float totalHeight = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxHeight = 0;  // Used to normalize result to 0-1 range

        for (int i = 0; i < octaves; i++)
        {
            totalHeight += Mathf.PerlinNoise(x * frequency / scale, z * frequency / scale) * amplitude;

            maxHeight += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return totalHeight / maxHeight;
    }

    void GenerateTerrain()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float y = GetHeight(x, z);

                // Instantiate the blocks based on y value
                if (y < 0.25f)
                {
                    Instantiate(waterBlock, new Vector3(x, y * scale, z), Quaternion.identity);
                }
                else if (y >= 0.25f && y < 0.5f)
                {
                    Instantiate(sandBlock, new Vector3(x, y * scale, z), Quaternion.identity);
                }
                else if (y >= 0.5f && y < 0.75f)
                {
                    Instantiate(grassBlock, new Vector3(x, y * scale, z), Quaternion.identity);
                }
                else
                {
                    Instantiate(snowBlock, new Vector3(x, y * scale, z), Quaternion.identity);
                }
            }
        }
    }
}