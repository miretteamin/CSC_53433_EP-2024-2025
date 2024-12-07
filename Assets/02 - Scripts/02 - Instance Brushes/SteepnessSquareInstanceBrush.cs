using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteepnessSquareInstanceBrush : InstanceBrush {


    protected float max_steepness = 30.0f;

    public override void draw(float x, float z) {
        int total_count = terrain.getObjectCount();
        // Debug.Log("Total count: " + total_count.ToString());

        float random_x = Random.Range(x - radius, x + radius);
        float random_z = Random.Range(z - radius, z + radius);

        float terrain_steepness = terrain.getSteepness(x, z);
        Debug.Log(terrain_steepness.ToString());        
        if (terrain_steepness < max_steepness)
        {
            spawnObject(random_x, random_z);
        }
    }
}
