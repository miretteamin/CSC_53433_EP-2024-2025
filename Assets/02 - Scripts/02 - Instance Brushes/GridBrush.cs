using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBrush : InstanceBrush
{
    public float gridSpacing = 2.0f; // Spacing between objects

    public override void draw(float x, float z)
    {
        int count = Mathf.CeilToInt(radius / gridSpacing);
        for (int i = -count; i <= count; i++)
        {
            for (int j = -count; j <= count; j++)
            {
                float offsetX = i * gridSpacing;
                float offsetZ = j * gridSpacing;
                spawnObject(x + offsetX, z + offsetZ);
            }
        }
    }
}
