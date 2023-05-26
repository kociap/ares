using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Entitiy entity;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        var movement = entity.getMovement();
        bool flipped = movement.x < 0;
        bool isMoving = movement != Vector2.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        animator.SetBool("Moving", isMoving);
    }
}
