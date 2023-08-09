using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Biome")]
public class Biome : ScriptableObject
{
    public string biomeName;
    public GameObject[] plantPrefabs;
    public float[] plantProbabilities; // Probabilities for each plant, should match the number of plantPrefabs
}