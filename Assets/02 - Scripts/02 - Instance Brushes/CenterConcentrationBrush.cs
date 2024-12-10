using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterConcentrationBrush : InstanceBrush
{

    public int totalInstances = 200; // Total instances to spawn
    public override void draw(float x, float z)
    {
        for (int i = 0; i < totalInstances; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius; // Square root ensures more points near center
            
            float fadeProbability = 1 - (distance / radius); // Higher probability near the center, lower near edges
            
            if (Random.value <= fadeProbability) // Only spawn if within probability threshold
            {
                float offsetX = Mathf.Cos(angle) * distance;
                float offsetZ = Mathf.Sin(angle) * distance;
                spawnObject(x + offsetX, z + offsetZ);
            }
        }
    }
}
