using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VolumePreservingBrush : TerrainBrush {
    public override void draw(int x, int z) {
        float totalHeight = 0;
        int count = 0;

        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                totalHeight += terrain.get(x + xi, z + zi);
                count++;
            }
        }

        float avgHeight = totalHeight / count;

        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                terrain.set(x + xi, z + zi, avgHeight); 
            }
        }
    }
}

