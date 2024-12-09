using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBrush : TerrainBrush {

    public float height = 5f;
    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float distance = Mathf.Sqrt(xi * xi + zi * zi);
                if (distance <= radius) {
                    terrain.set(x + xi, z + zi, height);
                }
            }
        }
    }
}

