using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneralNPC))]
public class NPCBehaviour : MonoBehaviour
{
    private enum NPCState {NOT_SYMULATED, IDDLE, APPROACH, ESCAPE}
    private NPCState state = NPCState.NOT_SYMULATED;
    private GeneralNPC npc;
    public static System.Random random = new System.Random();

    public float maxDistanceToPlayer = 20;
    public float minDistanceToPlayer = 10;
    public float simulationDistance = 30;

    public void Start()
    {
        npc = GetComponent<GeneralNPC>();
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case NPCState.NOT_SYMULATED:
                {
                    if (!PlayerToFar(simulationDistance))
                    {
                        state = NPCState.IDDLE;
                    }
                    break;
                }
            case NPCState.IDDLE:
                {
                    npc.SetMovement(new Vector2((float)random.Next(100) - 50, (float)random.Next(100) - 50).normalized);
                    checkSymulationDistance();
                    break;
                }
            case NPCState.APPROACH:
                {
                    //not implemented
                    checkSymulationDistance();
                    break;
                }
            case NPCState.ESCAPE:
                {
                    //not implemented
                    checkSymulationDistance();
                    break;
                }

        }
    }

    private void checkSymulationDistance()
    {
        if (PlayerToFar(simulationDistance))
        {
            state = NPCState.NOT_SYMULATED;
        }
    }

    private bool PlayerToFar(float maxDistance)
    {
        foreach (Player player in Player.instances)
        {
            if (Vector2.Distance(player.transform.position, transform.position) < maxDistance)
                return false;
        }
        return true;
    }
}
