using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Floor tile class, responsible for cracks on floor
[RequireComponent(typeof(Animator))]
public class Floor : MonoBehaviour
{
    // Tag present in every object that cracks floor
    public string heavyObjectTag;

    private Animator animator;

    // Min destruction value of floor (in range from 0 to 3) 
    private int minDestruction;
    // Current value of destruction
    private int destruction;

    // Max amount of frames between repairs
    public int maxRepairTime = 1000;
    // Regulates how much full repair time can vary in each repair
    public int repairTimeRange = 100;
    // Amount of frames left to repair
    private int timeToRepair;

    private void Start()
    {
        // Randomly initialize the minimum destruction level
        minDestruction = Random.Range(0, 3);

        // Set the initial destruction level to the minimum destruction
        destruction = minDestruction;

        // Get the Animator component attached to the game object
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Set the "Destruction" parameter in the Animator to the destruction level
        animator.SetInteger("Destruction", destruction);
    }

    private void FixedUpdate()
    {
        // Decrease the time to repair if it is greater than 0
        if (timeToRepair > 0)
        {
            timeToRepair--;
        }

        // Decrease the destruction level if it is greater than the minimum destruction and timeToRepair is less than or equal to 0
        if (destruction > minDestruction && timeToRepair <= 0)
        {
            destruction--;

            // Reset the timeToRepair with a new random value within the specified range
            timeToRepair = maxRepairTime + Random.Range(-repairTimeRange, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the tag "heavyObjectTag" and the destruction level is less than 3
        if (other.CompareTag(heavyObjectTag) && destruction < 3)
        {
            // Increase the destruction level by 1
            destruction++;

            // Set a new repair time by adding a random value between -repairTimeRange and 0 to the maximum repair time
            timeToRepair = maxRepairTime + Random.Range(-repairTimeRange, 0);
        }
    }

}
