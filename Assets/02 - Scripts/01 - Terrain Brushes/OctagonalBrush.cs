using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OctagonalBrush : TerrainBrush {
    public float height = 5f; 
    public float fadeIntensity = 0.5f;
    public int legs = 8;

    public override void draw(int x, int z) {
        float angleStep = Mathf.PI * 2 / legs; 
        Vector2 center = new Vector2(x, z);

        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                Vector2 point = new Vector2(x + xi, z + zi);
                Vector2 direction = point - center;
                float distance = direction.magnitude;

                if (distance > radius) continue; 

                float angle = Mathf.Atan2(direction.y, direction.x);
                float normalizedAngle = (angle + Mathf.PI * 2) % (Mathf.PI * 2); 

                bool withinLeg = false;
                for (int i = 0; i < legs; i++) {
                    float legAngle = i * angleStep;
                    float nextLegAngle = (i + 1) * angleStep;

                    if (normalizedAngle >= legAngle && normalizedAngle <= nextLegAngle) {
                        withinLeg = true;
                        break;
                    }
                }

                if (withinLeg) {
                    float fade = Mathf.Lerp(1f, fadeIntensity, distance / radius);
                    float heightAtPoint = height * fade;
                    terrain.set(x + xi, z + zi, heightAtPoint);
                }
            }
        }
    }
}
