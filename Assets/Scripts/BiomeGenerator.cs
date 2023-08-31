using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public int width = 100;
    public int depth = 100;
    public float scale = 20;
    public float moistureScale = 30;

    public GameObject mountainPrefab;
    public GameObject forestPrefab;
    public GameObject desertPrefab;
    public GameObject grasslandPrefab;
    public GameObject beachPrefab;
    public GameObject oceanPrefab;

    private void Start()
    {
        GenerateBiomes();
    }

    void GenerateBiomes()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float altitude = Mathf.PerlinNoise(x / scale, z / scale);
                float moisture = Mathf.PerlinNoise((x + width) / moistureScale, (z + depth) / moistureScale);

                if (altitude > 0.8f)
                {
                    Instantiate(mountainPrefab, new Vector3(x, 0, z), Quaternion.identity);
                }
                else if (altitude > 0.6f)
                {
                    if (moisture < 0.33f)
                    {
                        Instantiate(desertPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    }
                    else if (moisture < 0.66f)
                    {
                        Instantiate(grasslandPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(forestPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    }
                }
                else if (altitude > 0.3f)
                {
                    Instantiate(beachPrefab, new Vector3(x, 0, z), Quaternion.identity);
                }
                else
                {
                    Instantiate(oceanPrefab, new Vector3(x, 0, z), Quaternion.identity);
                }
            }
        }
    }
}