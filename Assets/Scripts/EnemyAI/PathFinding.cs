using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : Entitiy
{
    enum State {IDLE, CHASING}
    private State state = State.IDLE;

    public float minimumDistanceFromPlayer;
    public LayerMask obstacle;

    public Gun gun;

    private float distance;
    private GameObject player;

    private Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if(player == null)
        {
            return;
        }
        switch(state)
        {
            case State.IDLE:
                IdleStateAction();
                break;
            case State.CHASING:
                ChasingStateAction();
                break;
        }
    }

    void IdleStateAction()
    {
        if (gun != null)
        {
            gun.enabled = false;
        }
        moveRandom();
        distance = Vector2.Distance(transform.position, player.transform.position);
        if(1 <= distance && playerInView())
        {
            state = State.CHASING;
        }
    }

    void ChasingStateAction()
    {
        if (gun != null)
        {
            gun.enabled = true;
        }
        moveTowardPlayer();
        distance = Vector2.Distance(transform.position, player.transform.position);
        if(1 > distance || !playerInView())
        {
            state = State.IDLE;
        }
    }

    bool playerInView()
    {
        if(distance > minimumDistanceFromPlayer)
        {
            return false;
        } 
        else if (Physics2D.Linecast(transform.position, player.transform.position, obstacle))
        {
            return false;
        }
        return true;
    }

    void moveTowardPlayer()
    {
        movement = player.transform.position - transform.position;
        movement = movement.normalized;
        // Debug.DrawLine (transform.position, player.transform.position, Color.red);
        Controller.UpdateParameters parameters = new Controller.UpdateParameters
        {
            movement = movement,
            dTime = Time.fixedDeltaTime,
            time = Time.time
        };
        controller.UpdateController(parameters);
        //transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * 1.5f * Time.deltaTime);
    }

    void moveRandom(){
        movement = Vector2.zero;
        // Vector2 newPosition;

        // GameObject.FindGameObjectWithTag("Player");

        // Debug.DrawLine (transform.position, newPosition.transform.position, Color.red);
        // transform.position = Vector2.MoveTowards(this.transform.position, newPosition, speed * Time.deltaTime);
    }
}
