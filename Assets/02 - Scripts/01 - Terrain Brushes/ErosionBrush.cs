using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionBrush : TerrainBrush {
public float erosionFactor = 0.1f; 
    public float depositionFactor = 0.05f; 
    public int iterations = 3; 

    public override void draw(int x, int z) {
        for (int iteration = 0; iteration < iterations; iteration++) {
            for (int zi = -radius; zi <= radius; zi++) {
                for (int xi = -radius; xi <= radius; xi++) {
                    float currentHeight = terrain.get(x + xi, z + zi);
                    float lowestNeighborHeight = currentHeight;
                    int lowestNeighborX = 0, lowestNeighborZ = 0;

                    for (int dz = -1; dz <= 1; dz++) {
                        for (int dx = -1; dx <= 1; dx++) {
                            if (dx == 0 && dz == 0) continue;
                            float neighborHeight = terrain.get(x + xi + dx, z + zi + dz);
                            if (neighborHeight < lowestNeighborHeight) {
                                lowestNeighborHeight = neighborHeight;
                                lowestNeighborX = xi + dx;
                                lowestNeighborZ = zi + dz;
                            }
                        }
                    }

                    if (lowestNeighborHeight < currentHeight) {
                        float heightDifference = currentHeight - lowestNeighborHeight;
                        float erosionAmount = heightDifference * erosionFactor;
                        float depositionAmount = erosionAmount * depositionFactor;

                        terrain.set(x + xi, z + zi, currentHeight - erosionAmount);
                        terrain.set(x + lowestNeighborX, z + lowestNeighborZ, lowestNeighborHeight + depositionAmount);
                    }
                }
            }
        }
    }
}
