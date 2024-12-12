using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Animal : MonoBehaviour
{

    [Header("Animal parameters")]
    public float swapRate = 0.01f;
    public float mutateRate = 0.01f;
    public float swapStrength = 10.0f;
    public float mutateStrength = 0.5f;
    public float maxAngle = 10.0f;
    public float animalScale;
    private int lifetime;

    [Header("Energy parameters")]
    public float maxEnergy = 10.0f;
    public float lossEnergy = 0.1f;
    public float gainEnergy = 10.0f;
    public float energy;
    private int generation;
    private bool death;
    private int corpseCounter;
    private int eatingFrameCounter;
    private int procreationFrameCounter;

    [Header("Grass Counter parameters")]
    public int maxGrassCount = 10;
    private int grassCount = 0;

    [Header("Sensor - Vision")]
    public float maxVision = 20.0f;
    public float stepAngle = 10.0f;
    public int nEyes = 5;

    [Header("Rays")]
    public float rayScale;
    // public Color visionRayColor;

    private int[] networkStruct;
    private SimpleNeuralNet brain = null;

    // Terrain.
    private CustomTerrain terrain = null;
    private int[,] details = null;
    private Vector2 detailSize;
    private Vector2 terrainSize;

    // Animal.
    private Transform tfm;
    private float[] vision;

    // Genetic alg.
    private GeneticAlgo genetic_algo = null;

    // Renderer.
    private Material mat = null;
    public GameObject water;

    void Start()
    {

        lifetime = 0;

        // Network: 1 input per receptor, 1 output per actuator.
        vision = new float[nEyes + 1];
        networkStruct = new int[] { nEyes + 1, 5, 1 };
        energy = maxEnergy;
        tfm = transform;
        tfm.localScale = new Vector3(animalScale, animalScale, animalScale);

        // Renderer used to update animal color.
        // It needs to be updated for more complex models.
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
            mat = renderer.material;

        death = false;
        corpseCounter = -1;
        eatingFrameCounter = -1;

    }
    public void kill()
    {
        death = true;
        genetic_algo.removeAnimal(this);
    }
    void Update()
    {
        // tfm.position.x = 105f;
        // tfm.position.y = 133.17f;
        // tfm.position.z = 310f;
        tfm.position.Set(105f, 133.17f, 310f);
        lifetime++;
        if (lifetime > genetic_algo.getMaxLifetime())
            genetic_algo.setMaLifetime(lifetime);
        // In case something is not initialized...
        if (brain == null)
            brain = new SimpleNeuralNet(networkStruct);
        if (terrain == null)
            return;
        if (details == null)
        {
            UpdateSetup();
            return;
        }

        // If the Animal is dead
        if (death)
        {
            corpseCounter--;
            if (corpseCounter < 0)
            {
                // Remove corpse
                genetic_algo.removeAnimal(this);
            }
            return;
        }

        // Retrieve animal location in the heighmap
        int dx = (int)((tfm.position.x / terrainSize.x) * detailSize.x);
        int dy = (int)((tfm.position.z / terrainSize.y) * detailSize.y);

        // For each frame, we lose lossEnergy
        energy -= lossEnergy;

        // If the animal is located in the dimensions of the terrain and over a grass position (details[dy, dx] > 0), it eats it, gain energy and spawn an offspring.
        if ((dx >= 0) && dx < (details.GetLength(1)) && (dy >= 0) && (dy < details.GetLength(0)) && details[dy, dx] > 0)
        {
            // Eat (remove) the grass and gain energy.
            details[dy, dx] = 0;
            // Decrement count of grass
            genetic_algo.decrementGrass();
            if (genetic_algo.getHighlightEating())
            {
                eatingFrameCounter = 50;
            }
            // Add energy gain
            energy += gainEnergy;
            if (energy > maxEnergy)
                energy = maxEnergy;

            if (grassCount == maxGrassCount)
            {
                genetic_algo.addOffspring(this, generation + 1);
                grassCount = 0;
                if (genetic_algo.getHighlightProcreation())
                {
                    procreationFrameCounter = 50;
                }
            }
            else
            {
                grassCount += 1;
            }
        }

        // If the energy is below 0, the animal dies.
        if (energy < 0)
        {
            energy = 0.0f;
            death = true;
            if (genetic_algo.getShowCorpses())
            {
                // Number of frames the corpse will stay on screen
                corpseCounter = 50;

                // Immobilize Animal
                CapsuleAutoController cac = GetComponent<CapsuleAutoController>();
                cac.setMaxSpeed(0f);
            }
            else
            {
                genetic_algo.removeAnimal(this);
            }
        }

        // Update the color of the animal as a function of the energy that it contains.
        UpdateColor();

        // 1. Update receptor.
        UpdateVision();

        // 2. Use brain.
        float[] output = brain.getOutput(vision);

        // 3. Act using actuators.
        float angle = (output[0] * 2.0f - 1.0f) * maxAngle;
        tfm.Rotate(0.0f, angle, 0.0f);
    }

    /// <summary>
    /// Calculate distance to the nearest food resource, if there is any.
    /// </summary>
    private void UpdateVision()
    {
        float startingAngle = -((float)nEyes / 2.0f) * stepAngle;
        Vector2 ratio = detailSize / terrainSize;

        for (int i = 0; i < nEyes; i++)
        {
            Quaternion rotAnimal = tfm.rotation * Quaternion.Euler(0.0f, startingAngle + (stepAngle * i), 0.0f);
            Vector3 forwardAnimal = rotAnimal * Vector3.forward;
            float sx = tfm.position.x * ratio.x;
            float sy = tfm.position.z * ratio.y;
            vision[i] = 1.0f;
            if (genetic_algo.getDrawRays())
            {

                Debug.DrawRay(tfm.position,
                forwardAnimal.normalized * maxVision * rayScale,
                new Color(0f, 0.5f, 0f));
            }

            // Interate over vision length.
            for (float distance = 1.0f; distance < maxVision; distance += 0.5f)
            {
                // Position where we are looking at.
                float px = (sx + (distance * forwardAnimal.x * ratio.x));
                float py = (sy + (distance * forwardAnimal.z * ratio.y));

                if (px < 0)
                    px += detailSize.x;
                else if (px >= detailSize.x)
                    px -= detailSize.x;
                if (py < 0)
                    py += detailSize.y;
                else if (py >= detailSize.y)
                    py -= detailSize.y;

                if ((int)px >= 0 && (int)px < details.GetLength(1) && (int)py >= 0 && (int)py < details.GetLength(0) && details[(int)py, (int)px] > 0)
                {
                    vision[i] = distance / maxVision;
                    break;
                }
            }
        }
        // Vector3 waterPos = water.transform.position;
        // Debug.Log("waterpos: " + waterPos.ToString());


    }

    private void UpdateColor()
    {
        if (mat != null)
        {
            mat.SetFloat("_Metallic", 1.0f);
            if (death)
            {
                mat.color = genetic_algo.getDeathColor();
            }
            else if (procreationFrameCounter > 0)
            {
                mat.color = genetic_algo.getProcreatingColor();
                procreationFrameCounter--;
            }
            else if (eatingFrameCounter > 0)
            {
                mat.color = genetic_algo.getEatingColor();
                eatingFrameCounter--;
            }

            else
            {
                float health = energy / maxEnergy;
                mat.color = health * genetic_algo.getHealthyColor() + (1 - health) * genetic_algo.getDeathColor();
            }
        }
    }
    public void Setup(CustomTerrain ct, GeneticAlgo ga, int generation)
    {
        terrain = ct;
        genetic_algo = ga;
        this.generation = generation;
        UpdateSetup();
    }

    private void UpdateSetup()
    {
        detailSize = terrain.detailSize();
        Vector3 gsz = terrain.terrainSize();
        terrainSize = new Vector2(gsz.x, gsz.z);
        details = terrain.getDetails();
    }

    public void InheritBrain(SimpleNeuralNet other, bool mutate)
    {
        brain = new SimpleNeuralNet(other);
        if (mutate)
            brain.mutate(swapRate, mutateRate, swapStrength, mutateStrength);
    }
    public SimpleNeuralNet GetBrain()
    {
        return brain;
    }
    public float GetHealth()
    {
        return energy / maxEnergy;
    }

}
