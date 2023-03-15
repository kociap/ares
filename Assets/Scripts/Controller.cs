using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class Controller: MonoBehaviour {
    public struct UpdateParameters {
        public Vector2 movement;
        public float time;
        public float dTime;
    }

    [Header("Movement")]
    [SerializeField]
    private float acceleration = 1.0f;
    [SerializeField]
    private float maxVelocity = 1.0f;
    private Vector2 velocity;

    [Header("Collisions")]
    [SerializeField]
    private Bounds bounds;
    [SerializeField]
    private float collisionPrecheckLength = 0.1f;
    [SerializeField]
    private int collisionIterations = 10;
    private bool collisionUp = false;
    private bool collisionDown = false;
    private bool collisionLeft = false;
    private bool collisionRight = false;

    [Header("Debug")]
    [SerializeField]
    private bool drawDebugShapes = false;

    /// UpdateController
    /// Attempts to alter the state of the character according to the supplied
    /// parameters.
    ///
    /// Parameters:
    /// parameters - parameters for the controller update.
    ///
    public void UpdateController(UpdateParameters parameters) {
        CheckCollisions(parameters);
        CalculateMovement(parameters);
        Move(parameters);
    }

    private void CheckCollisions(UpdateParameters parameters) {
        Vector2 position = transform.position;
        collisionUp = TestOverlap(position + Vector2.up * collisionPrecheckLength);
        collisionDown = TestOverlap(position + Vector2.down * collisionPrecheckLength);
        collisionLeft = TestOverlap(position + Vector2.left * collisionPrecheckLength);
        collisionRight = TestOverlap(position + Vector2.right * collisionPrecheckLength);
    }

    private void CalculateMovement(UpdateParameters parameters) {
        // We gradually accelerate or decelerate until max speed is reached.
        float dTime = parameters.dTime;
        Vector2 movement = parameters.movement;
        if(movement.x != 0) {
            velocity.x += Mathf.Sign(movement.x) * acceleration * dTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
        } else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0.0f, acceleration * dTime);
        }

        // If collision has been detected, zero the velocity. This allows us to
        // slide along a wall.
        if(velocity.x > 0 && collisionRight || velocity.x < 0 && collisionLeft) {
            velocity.x = 0;
        }

        // Identical to the X axis.
        if(movement.y != 0) {
            velocity.y += Mathf.Sign(movement.y) * acceleration * dTime;
            velocity.y = Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity);
        } else {
            velocity.y = Mathf.MoveTowards(velocity.y, 0.0f, acceleration * dTime);
        }

        if(velocity.y > 0 && collisionUp || velocity.y < 0 && collisionDown) {
            velocity.y = 0;
        }
    }

    private void Move(UpdateParameters parameters) {
        // Perform box overlaps incrementally moving the collision box closer
        // toward the farthest reachable point.
        Vector2 displacement = velocity * parameters.dTime;
        Vector2 startingPosition = transform.position;
        Vector2 farthestPosition = startingPosition + displacement;
        Vector2 finalPosition = startingPosition;
        for(int iteration = 1; iteration <= collisionIterations; iteration += 1) {
            float t = (float)iteration / (float)collisionIterations;
            Vector2 nextPosition = Vector2.Lerp(startingPosition, farthestPosition, t);
            if(TestOverlap(nextPosition)) {
                break;
            }
            finalPosition = nextPosition;
        }
        transform.position = finalPosition;
    }

    /// TestOverlap
    /// Test whether the bounds of the controller overlap any colliders at
    /// a position.
    ///
    /// Parameters:
    /// position - position to test the overlap.
    ///
    /// Returns:
    /// true if the bounds overlap any colliders.
    ///
    private bool TestOverlap(Vector2 position) {
        Vector2 center = bounds.center;
        Collider2D collider = Physics2D.OverlapBox(position + center, bounds.size, 0);
        return collider != null;
    }

    private void OnDrawGizmos() {
        if(!drawDebugShapes) {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
    }
}
