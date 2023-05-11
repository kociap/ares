using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(GeneralNPC))]
public class NPCBehaviour : MonoBehaviour
{
    private enum NPCState {NOT_SYMULATED, IDDLE}
    private NPCState state = NPCState.NOT_SYMULATED;
    private GeneralNPC npc;
    public static System.Random random = new System.Random();

    public float maxDistanceToPlayer = 20;
    public float minDistanceToPlayer = 10;
    public float simulationDistance = 30;

    private int MaxframesToChangeDirection = 10;
    private int framesToChangeDirection = 0;

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
                    ActivateNPC();
                    break;
                }
            case NPCState.IDDLE:
                {
                    randomWandering();
                    checkSymulationDistance();
                    break;
                }
        }
    }

    private void ActivateNPC()
    {
        if (!PlayerToFar(simulationDistance))
        {
            state = NPCState.IDDLE;
            framesToChangeDirection = 0;
        }
    }

    private void randomWandering()
    {
        if (framesToChangeDirection == 0)
        {
            npc.SetMovement(getRandomVerctor());
            framesToChangeDirection = MaxframesToChangeDirection;
        }
        else
        {
            framesToChangeDirection--;
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

    private Vector2 getRandomVerctor()
    {
        //todo: improve, refactor
        List<Vector2> directions = new List<Vector2>();
        bool[] collisons = npc.GetController().getColisions();
        if (!collisons[3] && !collisons[0] && !collisons[1]) directions.Add(Vector2.right);
        if (!collisons[0] && !collisons[1] && !collisons[2]) directions.Add(Vector2.up);
        if (!collisons[1] && !collisons[2] && !collisons[3]) directions.Add(Vector2.left);
        if (!collisons[2] && !collisons[3] && !collisons[0]) directions.Add(Vector2.down);
        if(collisons[0] && collisons[1] && collisons[2] && collisons[3])
        {
            directions.AddRange(new Vector2[] { Vector2.right, Vector2.up, Vector2.left, Vector2.down });
        }
        return directions[random.Next(directions.Count)];
    }
}
