using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareInstanceBrush : InstanceBrush {
    

    public override void draw(float x, float z) {

        float random_x = Random.Range(x - radius, x + radius);
        float random_z = Random.Range(z - radius, z + radius);
        spawnObject(random_x, random_z);
    }
}
