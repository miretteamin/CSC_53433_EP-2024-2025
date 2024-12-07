using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalBrush : TerrainBrush {

    public float delta = 1;

    

    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {

                Debug.Log("terrain.get(0, 0)");

                float height = terrain.get(x + xi, z + zi) + delta;
                terrain.set(x + xi, z + zi, height);
            }
        }
    }
}
