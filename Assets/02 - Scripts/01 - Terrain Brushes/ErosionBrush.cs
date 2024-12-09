using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionBrush : TerrainBrush {
    public float erosionFactor = 0.1f;

    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float currentHeight = terrain.get(x + xi, z + zi);
                terrain.set(x + xi, z + zi, currentHeight - erosionFactor); 
            }
        }
    }
}

