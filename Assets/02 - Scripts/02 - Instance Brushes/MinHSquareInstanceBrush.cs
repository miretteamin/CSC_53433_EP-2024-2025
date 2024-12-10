using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHSquareInstanceBrush : InstanceBrushMultiObj {

    public override void draw(float x, float z) {
        int total_count = terrain.getObjectCount();
        // Debug.Log("Total count: " + total_count.ToString());

        float random_x = Random.Range(x - radius, x + radius);
        float random_z = Random.Range(z - radius, z + radius);

        for (int i = 0; i < total_count; i++){
            Vector3 location = terrain.getObjectLoc(i);
            double other_x = location[0];
            double other_z = location[2];
            double diff_x = random_x - other_x;
            double diff_z = random_z - other_z;
            double distance = System.Math.Sqrt((diff_x * diff_x) + (diff_z * diff_z));

            if (distance <= terrain.brush_radius)
            {
                return;
            }

        }
       
        spawnObject(random_x, random_z);
    }
}
