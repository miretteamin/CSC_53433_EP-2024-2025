using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GeneticAlgo : MonoBehaviour
{

    [Header("Genetic Algorithm parameters")]
    [Range(0, 1000)]
    public int popSize = 100;
    public GameObject animalPrefab;

    [Header("Actions")]
    [Range(1, 5)]
    public int task = 1;

    public bool drawRays;

    public bool clearTerrain;
    public bool fillTerrain;
    public bool updateRandom;
    public bool saveStatisticsToDisk = false;

    [Header("Grass Clusters")]
    public bool makeClusters;
    public int numClusters = 4;
    [Range(0, 200)]
    public float clusterRadius = 50f;

    [Range(0, 1)]
    public float decayRatio = 0.90f;
    [Range(0, 1)]
    public float rockCoverageRate = 0.005f;
    public float grassCoverageRate = 0.6f;
    public float snowCoverageRate = 0.0f;
    private float currentClusters;
    private float _numClusters;

    [Header("Debug Text")]
    public bool showAnimals = true;
    public bool showGrassCount = true;
    public bool showFrame = true;
    public bool showMaxGeneration = true;
    public bool showMaxLifetime = true;
    private int maxGeneration;
    private int maxLifetime;

    [Header("Animal Colors")]
    public Color healthyColor;
    public Color deathColor;
    public Color eatingColor;
    public Color procreatingColor;
    public bool showCorpses = true;
    public bool highlightEating = true;
    public bool highlightProcreation = true;

    [Header("Dynamic elements")]
    public float vegetationGrowthRate = 0.01f;
    public float currentGrowth;

    float[,,] alphamaps;

    public float scale = 0.05f;

    // Number of grass elements in the terrain
    private int grassCount;

    private int frame;

    private List<GameObject> animals;
    protected Terrain terrain;
    protected CustomTerrain customTerrain;
    protected float width;
    protected float height;

    [Header("Statistics")]

    private List<int> animalStats;
    private List<int> grassStats;
    private List<int> maxGenerationStats;
    private List<int> maxLifetimeStats;

    const int SNOW = 1;
    const int GRASS = 2;
    const int ROCK = 3;

    [Header("Factory")]

    public bool dumpWaste;
    private bool isToxic;
    public GameObject water;
    // public Water water;
    void Start()
    {

        // Retrieve terrain.
        terrain = Terrain.activeTerrain;
        customTerrain = GetComponent<CustomTerrain>();
        width = terrain.terrainData.size.x;
        height = terrain.terrainData.size.z;

        // Initialize terrain growth.
        currentGrowth = 0.0f;

        // Initialize arrays
        animals = new List<GameObject>();
        animalStats = new List<int>();
        grassStats = new List<int>();
        maxGenerationStats = new List<int>();
        maxLifetimeStats = new List<int>();
        dumpWaste = false;
        isToxic = false;

        // Clear Terrain
        clearTerrainFn();

        frame = 0;
        showAnimals = true;
        showGrassCount = true;
        showFrame = true;
        currentClusters = 0f;
        _numClusters = (float)numClusters;
        maxGeneration = 0;
        maxLifetime = 0;
        saveStatisticsToDisk = false;

        alphamaps = customTerrain.getTextures();

        if (task == 1)
        {
            // checkpoint
            makeClustersFn(numClusters);
            fillTerrain = false;
            updateRandom = false;
            showGrassCount = true;
            showAnimals = false;
            showMaxGeneration = false;
            showFrame = true;
        }
        else if (task == 2)
        {
            makeClustersFn(numClusters);
            for (int i = 0; i < popSize; i++)
            {
                GameObject animal = makeAnimal(1);
                animals.Add(animal);
            }
            makeClusters = true;
            drawRays = true;
            showGrassCount = true;
            showAnimals = true;
            showMaxGeneration = true;
            showFrame = true;
        }
        else if (task == 3)
        {
            //IK    
            makeClusters = false;
            popSize = 0;
            drawRays = false;
            showGrassCount = false;
            showAnimals = false;
            showMaxGeneration = false;
            showMaxLifetime = false;
            showFrame = true;
        }
    }

    void Update()
    {

        // Increment frame
        frame++;
        // Keeps animal to a minimum.
        // while (animals.Count < popSize / 2)
        // {
        //     animals.Add(makeAnimal());
        // }
        updateDebugText();
        // Update grass elements/food resources.
        updateResources();

        updateStats();

        if (saveStatisticsToDisk)
        {
            saveToDisk();
        }

        if (dumpWaste)
        {
            isToxic = true;
            dumpWasteFn();
        }

        // Decay num clusters over time
        // if (makeClusters)
        //     _numClusters *= decayRatio;
    }

    private void dumpWasteFn()
    {
        WaterMaterial waterMat = water.GetComponent<WaterMaterial>();
        waterMat.toxify();

    }
    public bool getToxicity() { return isToxic; }
    private void updateStats()
    {

        animalStats.Add(animals.Count);
        grassStats.Add(grassCount);
        maxGenerationStats.Add(maxGeneration);
        maxLifetimeStats.Add(maxLifetime);

    }

    private void saveToDisk()
    {
        string directoryPath = "./Statistics";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string animalsFilePath = Path.Combine(directoryPath, "animals.csv");
        string grassFilePath = Path.Combine(directoryPath, "grass.csv");
        string maxGenerationFilePath = Path.Combine(directoryPath, "max_generation.csv");
        string maxLifetimeFilePath = Path.Combine(directoryPath, "max_lifetime.csv");
        string tstFilePath = Path.Combine(directoryPath, "tst.csv");

        SaveArrayToCSV(animalStats, animalsFilePath);
        SaveArrayToCSV(grassStats, grassFilePath);
        SaveArrayToCSV(maxGenerationStats, maxGenerationFilePath);
        SaveArrayToCSV(maxLifetimeStats, maxLifetimeFilePath);

        Debug.Log("Saved Statistics to Disk");
        saveStatisticsToDisk = false;
    }
    void SaveArrayToCSV(List<int> array, string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            // Write array as a single line of comma-separated values
            writer.WriteLine(string.Join(",", array));
        }
    }
    void SaveArrayToCSV(List<float> array, string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            // Write array as a single line of comma-separated values
            writer.WriteLine(string.Join(",", array));
        }
    }
    private void updateDebugText()
    {
        customTerrain.debug.text = "";
        if (showAnimals)
            customTerrain.debug.text += "\nNum Animals: " + animals.Count.ToString();
        if (showGrassCount)
            customTerrain.debug.text += "\nNum Grass: " + grassCount.ToString();
        if (showMaxGeneration)
            customTerrain.debug.text += "\nMax Generation: " + maxGeneration.ToString();
        if (showMaxLifetime)
            customTerrain.debug.text += "\nMax Lifetime: " + maxLifetime.ToString();
        if (showFrame)
            customTerrain.debug.text += "\nFrame: " + frame.ToString();
        while (customTerrain.debug.text.Length > 0 && customTerrain.debug.text[0] == '\n')
            customTerrain.debug.text = customTerrain.debug.text.Substring(1);

    }

    /// <summary>
    /// Method to place grass or other resource in the terrain.
    /// </summary>
    public void updateResources()
    {
        if (clearTerrain)
        {
            clearTerrainFn();
            clearTerrain = false;
            return;
        }
        else if (fillTerrain)
        {
            fillTerrainFn();
            fillTerrain = false;
            return;

        }
        else if (makeClusters)
        {
            makeClustersFn();
            return;
        }
        else if (updateRandom)
        {
            updateResourcesRandom();
        }
    }
    public void createCluster(int x, int y, int texture)
    {
        Vector2 detail_sz = customTerrain.detailSize();
        int[,] details = customTerrain.getDetails();
        int startX = (int)Math.Max(0, (float)x - 2 * clusterRadius);
        int endX = (int)Math.Min(detail_sz.x, (float)x + 2 * clusterRadius);
        int startY = (int)Math.Max(0, (float)y - 2 * clusterRadius);
        int endY = (int)Math.Min(detail_sz.y, (float)y + 2 * clusterRadius);

        float coverageRate = 0.0f;
        if (texture == ROCK)
        {
            coverageRate = rockCoverageRate;
        }
        else if (texture == GRASS)
        {
            coverageRate = grassCoverageRate;
        }
        else if (texture == SNOW)
        {
            coverageRate = snowCoverageRate;
        }
        else
        {
            coverageRate = 0.0f;
        }
        // Debug.Log("(" + x.ToString() + ", " + y.ToString() + ")");
        // Debug.Log("Texture: " + texture.ToString());
        coverageRate = 0.35f;

        for (int i = startX; i < endX; i++)
        {
            for (int j = startY; j < endY; j++)
            {
                if (Math.Sqrt((x - i) * (x - i) + (y - j) * (y - j)) <= clusterRadius)
                {
                    if (UnityEngine.Random.value < coverageRate)
                    {
                        if (details[j, i] != 1)
                        {
                            grassCount++;
                        }
                        details[j, i] = 1;
                    }
                }
            }
        }
    }
    public void makeClustersFn(int numClustersToCreate)
    {
        Vector2 detail_sz = customTerrain.detailSize();
        int[,] details = customTerrain.getDetails();
        for (int cluster = 0; cluster < numClustersToCreate; cluster++)
        {
            int x = (int)(UnityEngine.Random.value * detail_sz.x);
            int y = (int)(UnityEngine.Random.value * detail_sz.y);

            float[] texture = new float[6];
            for (int i = 0; i < alphamaps.GetLength(2); i++)
            {
                texture[i] = alphamaps[(int)(x / 2), (int)(y / 2), i];
            }
            int argMaxTexture = argmax(texture);
            createCluster(x, y, argMaxTexture);
        }
        customTerrain.saveDetails();
    }
    private int argmax(float[] array)
    {
        int maxIndex = 0;
        float maxValue = array[0];

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > maxValue)
            {
                maxValue = array[i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }
    public void makeClustersFn()
    {
        // Debug.Log("Num clusters: " + _numClusters.ToString());
        // Debug.Log("CurrGrass: " + ((float)grassCount / 1257f).ToString());
        while (_numClusters > ((float)grassCount / 1257f))
        {
            makeClustersFn(1);
        }
        // Debug.Log("New: " + grassCount.ToString());
    }
    public void clearTerrainFn()
    {
        Vector2 detail_sz = customTerrain.detailSize();
        int[,] details = customTerrain.getDetails();
        for (int i = 0; i < detail_sz.x; i++)
        {
            for (int j = 0; j < detail_sz.y; j++)
            {
                details[j, i] = 0;
            }
        }
        customTerrain.saveDetails();
        grassCount = 0;
        clearTerrain = false;
    }
    public void fillTerrainFn()
    {
        Vector2 detail_sz = customTerrain.detailSize();
        int[,] details = customTerrain.getDetails();
        for (int i = 0; i < detail_sz.x; i++)
        {
            for (int j = 0; j < detail_sz.y; j++)
            {
                if (details[j, i] != 1)
                    grassCount++;
                details[j, i] = 1;
            }
        }
        customTerrain.saveDetails();
    }
    public void updateResourcesRandom()
    {
        float threshold = 0.5f;
        Vector2 detail_sz = customTerrain.detailSize();
        int[,] details = customTerrain.getDetails();
        currentGrowth += vegetationGrowthRate;

        while (currentGrowth > 1.0f)
        {
            for (int x = 0; x < detail_sz.x; x++)
            {
                for (int y = 0; y < detail_sz.y; y++)
                {
                    float prob = Mathf.PerlinNoise((float)x * scale, (float)y / scale);
                    if (prob > 0.8f)
                    {
                        prob = 0.8f;
                    }
                    if (UnityEngine.Random.value < prob)
                    {
                        if (details[y, x] != 1)
                            grassCount++;
                        details[y, x] = 1;
                    }
                }
            }
            // int x = (int)(UnityEngine.Random.value * detail_sz.x);
            // int y = (int)(UnityEngine.Random.value * detail_sz.y);
            // details[y, x] = 1;
            currentGrowth -= 1.0f;
        }
        customTerrain.saveDetails();
    }

    /// <summary>
    /// Method to instantiate an animal prefab. It must contain the animal.cs class attached.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject makeAnimal(Vector3 position, int generation)
    {
        GameObject animal = Instantiate(animalPrefab, transform);
        animal.GetComponent<Animal>().Setup(customTerrain, this, generation);
        animal.transform.position = position;
        animal.transform.Rotate(0.0f, UnityEngine.Random.value * 360.0f, 0.0f);
        return animal;
    }

    /// <summary>
    /// If makeAnimal() is called without position, we randomize it on the terrain.
    /// </summary>
    /// <returns></returns>
    public GameObject makeAnimal(int generation)
    {
        Vector3 scale = terrain.terrainData.heightmapScale;
        float x = UnityEngine.Random.value * width;
        float z = UnityEngine.Random.value * height;
        float y = customTerrain.getInterp(x / scale.x, z / scale.z);
        return makeAnimal(new Vector3(x, y, z), generation);
    }

    /// <summary>
    /// Method to add an animal inherited from anothed. It spawns where the parent was.
    /// </summary>
    /// <param name="parent"></param>
    public void addOffspring(Animal parent, int generation)
    {
        if (generation > maxGeneration)
            maxGeneration = generation;
        GameObject animal = makeAnimal(parent.transform.position, generation);
        animal.GetComponent<Animal>().InheritBrain(parent.GetBrain(), true);
        animals.Add(animal);
    }

    /// <summary>
    /// Remove instance of an animal.
    /// </summary>
    /// <param name="animal"></param>
    public void removeAnimal(Animal animal)
    {
        animals.Remove(animal.transform.gameObject);
        Destroy(animal.transform.gameObject);
    }

    public bool getDrawRays()
    {
        return drawRays;
    }

    public void decrementGrass()
    {
        grassCount--;
    }
    public Color getHealthyColor()
    {
        return healthyColor;
    }

    public Color getDeathColor()
    {
        return deathColor;
    }
    public Color getEatingColor()
    {
        return eatingColor;
    }
    public Color getProcreatingColor()
    {
        return procreatingColor;
    }
    public bool getShowCorpses()
    {
        return showCorpses;
    }
    public bool getHighlightEating()
    {
        return highlightEating;
    }
    public bool getHighlightProcreation()
    {
        return highlightProcreation;
    }
    public int getMaxLifetime()
    {
        return maxLifetime;
    }
    public void setMaLifetime(int maxLifetime)
    {
        this.maxLifetime = maxLifetime;
    }
}
