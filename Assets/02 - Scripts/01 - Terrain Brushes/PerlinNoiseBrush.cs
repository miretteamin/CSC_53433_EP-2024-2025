using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PerlinNoiseBrush : TerrainBrush {
    public float height = 5;
    public float scale = 2;
    public float intensity = 2;
    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float noiseValue = Mathf.PerlinNoise(
                        (x + xi) / 2, 
                        (z + zi) / 2
                    ) * 0.3f; // + Mathf.PerlinNoise(
                    //     (x + xi) / 1, 
                    //     (z + zi) / 1
                    // ) * 0.2f + Mathf.PerlinNoise(
                    //     (x + xi) / 6, 
                    //     (z + zi) / 6
                    // ) * 0.1f;
                
                
                terrain.set(x + xi, z + zi, height*noiseValue);
                
            }
        }
    }
}
;