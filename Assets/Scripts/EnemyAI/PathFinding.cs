using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public float speed;
    public float minimumDistanceFromPlayer;
    public LayerMask obstacle;

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
        distance = Vector2.Distance(transform.position, player.transform.position);

        if (1 <= distance && playerInView()) 
        {
            moveTowardPlayer();
        }
        else
        {
            moveRandom();
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
        Vector2 direction = player.transform.position - transform.position;
        // Debug.DrawLine (transform.position, player.transform.position, Color.red);
        Controller.UpdateParameters parameters = new Controller.UpdateParameters
        {
            movement = direction,
            dTime = Time.fixedDeltaTime,
            time = Time.time
        };
        controller.UpdateController(parameters);
        //transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * 1.5f * Time.deltaTime);
    }

    void moveRandom(){
        // Vector2 newPosition;

        // GameObject.FindGameObjectWithTag("Player");

        // Debug.DrawLine (transform.position, newPosition.transform.position, Color.red);
        // transform.position = Vector2.MoveTowards(this.transform.position, newPosition, speed * Time.deltaTime);
    }
}
