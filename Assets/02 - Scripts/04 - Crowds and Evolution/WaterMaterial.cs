using UnityEngine;

public class WaterMaterial : MonoBehaviour
{
    public Material toxicMaterial; // Assign a material in the Inspector

    void Start()
    {
        // Get the Renderer component of the GameObject
        Renderer renderer = GetComponent<Renderer>();


    }

    public void toxify()
    {
        if (GetComponent<Renderer>() != null)
        {
            // Change the material
            GetComponent<Renderer>().material = toxicMaterial;
        }
    }
}
