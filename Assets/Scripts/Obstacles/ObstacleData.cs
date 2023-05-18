using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle Data", menuName = "Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    public GameObject gameObject; // The obstacle GameObject
    public float chance = 10f; // The chance of the obstacle spawning
    
}