using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstanceBrushMultiObj : Brush {

    private int prefab_idx;
    private int[] prefab_idxs = {0, 2, 3, 4};

    public override void callDraw(float x, float z) {
        if (terrain.object_prefab)
            prefab_idx = terrain.registerPrefab(terrain.object_prefab);
        else {
            prefab_idx = -1;
            terrain.debug.text = "No prefab to instantiate";
            return;
        }
        Vector3 grid = terrain.world2grid(x, z);
        draw(grid.x, grid.z);
    }

    public override void draw(int x, int z) {
        draw((float)x, (float)z);
    }

    public void spawnObject(float x, float z) {
        if (prefab_idx == -1) {
            return;
        }
        prefab_idx = prefab_idxs[Random.Range(0, 4)];
        Debug.Log(prefab_idx);
        float scale_diff = Mathf.Abs(terrain.max_scale - terrain.min_scale);
        float scale_min = Mathf.Min(terrain.max_scale, terrain.min_scale);
        float scale = (float)CustomTerrain.rnd.NextDouble() * scale_diff + scale_min;
        terrain.spawnObject(terrain.getInterp3(x, z), scale, prefab_idx);
    }
}
