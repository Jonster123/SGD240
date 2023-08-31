using UnityEngine;

public class TerrainAndBiomeGenerator : MonoBehaviour
{
    public int width = 100;
    public int depth = 100;
    public float scale = 20;
    public float moistureScale = 30;

    public GameObject mountainBlock;
    public GameObject forestBlock;
    public GameObject desertBlock;
    public GameObject grasslandBlock;
    public GameObject beachBlock;
    public GameObject oceanBlock;
    public GameObject snowBlock; // from the previous script

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
            }
        }
    }

    GameObject DetermineBlockBasedOnBiomeAndAltitude(float altitude, float moisture)
    {
        if (altitude > 0.8f)
        {
            return snowBlock; // high altitudes have snow
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
}