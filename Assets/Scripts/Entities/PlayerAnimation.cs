using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Entitiy entity;
    private Animator animator;
    private Vector2 lastMovemnt;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        var movement = entity.getMovement();
        bool flipped = lastMovemnt.x < 0;
        bool isMoving = movement != Vector2.zero;
        if (isMoving) 
        {
            flipped = movement.x < 0;
            lastMovemnt = movement;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        animator.SetBool("Moving", isMoving);
    }
}
