using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothBrush : TerrainBrush {
    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float sum = 0;
                int cnt = 0;

                for (int offsetZ = -1; offsetZ <= 1; offsetZ++) {
                    for (int offsetX = -1; offsetX <= 1; offsetX++) {
                        sum += terrain.get(x + xi + offsetX, z + zi + offsetZ);
                        cnt++;
                    }
                }

                float avg = sum / cnt;
                terrain.set(x + xi, z + zi, avg); // avg to smooth
            }
        }
    }
}

