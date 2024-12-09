using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class BumpyNoisyBrush : TerrainBrush {
    public float bumpHeight = 5f;   
    public float scale = 0.1f;  
    public bool usePerlinNoise = true;

    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float distance = Mathf.Sqrt(xi * xi + zi * zi);
                if (distance <= radius) {
                    float currentHeight = terrain.get(x + xi, z + zi);
                    float bumpValue;

                    if (usePerlinNoise) {
                        bumpValue = Mathf.PerlinNoise((x + xi) * scale, (z + zi) * scale);
                    } 
                    else {
                        bumpValue = Random.Range(-1f, 1f);
                    }

                    float newHeight = currentHeight + bumpHeight * bumpValue;
                    terrain.set(x + xi, z + zi, newHeight);
                }
            }
        }
    }
}
