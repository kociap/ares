using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100;
    public float lifespan = 5;
    private Controller controller;
    Vector2 movement;

    void Start()
    {
        controller = GetComponent<Controller>();
        Destroy(gameObject, lifespan);
    }

    void FixedUpdate()
    {
        Move();
        //transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
    }

    public void SetMovement(Vector2 movement)
    {
        this.movement = movement;
    }

    public void Move()
    {
        controller = GetComponent<Controller>();
        Controller.UpdateParameters parameters = new Controller.UpdateParameters
        {
            movement = movement,
            dTime = Time.fixedDeltaTime,
            time = Time.time
        };
        controller.UpdateController(parameters);
    }
}
