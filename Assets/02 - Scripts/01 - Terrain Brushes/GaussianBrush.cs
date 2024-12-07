using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GaussianBrush : TerrainBrush {

    public float height = 5;
    public float alpha = 0.5f;

    public override void draw(int x, int z) {
        
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                // Console.WriteLine("Log");
                // Debug.Log("a7a" + (float)Math.Abs(xi) / (2.0 * (float)radius));
                // Debug.Log(Math.Abs(zi - z) / 2 / radius);
                
                double temp = -0.5 * (Math.Pow((float)(Math.Abs(xi) / alpha / (float)radius), 2) + Math.Pow((float)(Math.Abs(zi) / alpha / (float)radius), 2));
                // terrain.set(x + xi, z + zi, 1.0 / (2.0 * (float)Math.PI) * Math.Exp((float)temp)         );
                terrain.set(x + xi, z + zi, (float)(height / (2.0 * (float)Math.PI) * Math.Exp((float)temp)));
                Debug.Log((float)(height / (2.0 * (float)Math.PI) * Math.Exp((float)temp)));
                Debug.Log(temp);

                
                // Console.P((float)(x + xi), (float)(z + zi), (float)(1.0 / (2.0 * (float)Math.PI) * Math.Exp((float)temp)));
            }
        }
    }
}
