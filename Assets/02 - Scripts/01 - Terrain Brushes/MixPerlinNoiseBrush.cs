using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixPerlinNoiseBrush : TerrainBrush {
    public float height = 5;      
    public float scale = 2;       
    public float intensity = 2;   
    public int octaves = 3;       
    public float persistence = 0.5f;
    public float lacunarity = 2f;


    private float GeneratePerlinNoise(int x, int z) {
        float total = 0f;
        float frequency = scale;
        float amplitude = intensity;
        float maxAmplitude = 0f; 

        for (int i = 0; i < octaves; i++) {
            float sampleX = x / frequency;
            float sampleZ = z / frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);

            total += perlinValue * amplitude;
            maxAmplitude += amplitude;

            amplitude *= persistence; 
            frequency *= lacunarity;  
        }

        return total / maxAmplitude; 
    }


    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float noiseValue = GeneratePerlinNoise(x + xi, z + zi);
                terrain.set(x + xi, z + zi, height * noiseValue); 
            }
        }
    }
}
