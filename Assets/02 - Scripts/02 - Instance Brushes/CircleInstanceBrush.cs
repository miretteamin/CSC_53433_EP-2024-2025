using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleInstanceBrush : InstanceBrush {

    public override void draw(float x, float z) {
        float random_radius = Random.Range(0.0f, (float)radius);

        float angle = Random.Range(0f, 2f * Mathf.PI);
        spawnObject(x + random_radius * Mathf.Cos(angle), z + random_radius * Mathf.Sin(angle));
    }
}
