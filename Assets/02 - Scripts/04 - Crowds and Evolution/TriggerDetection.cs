using UnityEngine;

public class TriggerDetection : MonoBehaviour
{

    private bool isToxic;
    //public GameObject terrain;

    public string targetTag = "Ground"; // Set the tag in the Inspector
    public GameObject terrain;
    
    public void Start()
    {
        Debug.Log("Starting Trigger detector.");
        terrain = GameObject.FindWithTag(targetTag);
           

    }
    // Called when another collider enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Water")
        {
            Debug.Log("Collided with: " + other.tag);
            if (!isToxic)
            {
                terrain = GameObject.FindWithTag(targetTag);

                GeneticAlgo ga = terrain.GetComponent<GeneticAlgo>();
                isToxic = ga.getToxicity();
            }

            if (isToxic)
            {
                Animal animal = GetComponent<Animal>();
                animal.kill();
            }
        }


    }
}
