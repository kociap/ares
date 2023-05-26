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
    // Velocity represents the current direction of the velocity with length
    // subject to scale to support sub and sup max velocity speeds.
    private Vector2 velocity;
    private Vector2 lastVelocityDirection;

    public Vector2 GetVelocity() {
        return maxVelocity * velocity;
    }

    [Header("Dash")]
    [SerializeField]
    private float dashInitialVelocityFactor = 2.0f;
    [SerializeField]
    private float dashDuration = 1.0f;
    private float dashAcceleration = 0.5f;
    private Vector2 dashTargetVelocity;
    private float dashCurrentTime = 0.0f;
    private bool dashing = false;
    private bool startDashThisUpdate = false;

    public bool IsDashing() {
        return dashing;
    }

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

    public void BeginDash() {
        startDashThisUpdate = true;
        dashTargetVelocity = velocity;
        dashCurrentTime = dashDuration;
    }

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
        if(startDashThisUpdate) {
            startDashThisUpdate = false;
            dashing = true;
            velocity = lastVelocityDirection * dashInitialVelocityFactor;
            Debug.Log("last: " + lastVelocityDirection + "; target: " + dashTargetVelocity + "; starting: " + velocity);
            dashAcceleration = (velocity - dashTargetVelocity).magnitude / dashDuration;
        }

        if(dashing && dashCurrentTime <= 0.0f) {
            dashing = false;
        }

        // We gradually accelerate or decelerate until movement speed is reached.
        float dTime = parameters.dTime;
        Vector2 movement = parameters.movement;
        if(dashing) {
            // velocity is non-zero, hence we do not have worry about
            // multiplication with a zero vector.
            velocity = velocity.normalized * Mathf.MoveTowards(velocity.magnitude, dashTargetVelocity.magnitude, dashAcceleration * dTime);
            dashCurrentTime -= dTime;
        } else {
            Vector2 accelerationVector = acceleration * (movement - velocity).normalized;
            accelerationVector.x = Mathf.Abs(accelerationVector.x);
            accelerationVector.y = Mathf.Abs(accelerationVector.y);
            velocity.x = Mathf.MoveTowards(velocity.x, movement.x, accelerationVector.x * dTime);
            velocity.y = Mathf.MoveTowards(velocity.y, movement.y, accelerationVector.y * dTime);
        }

        // If collision has been detected, zero the velocity. This allows us to
        // slide along a wall.
        if(velocity.x > 0 && collisionRight || velocity.x < 0 && collisionLeft) {
            velocity.x = 0;
        }

        if(velocity.y > 0 && collisionUp || velocity.y < 0 && collisionDown) {
            velocity.y = 0;
        }

        if(velocity.sqrMagnitude > 0.01f) {
            lastVelocityDirection = velocity.normalized;
        }
    }

    private void Move(UpdateParameters parameters) {
        // Perform box overlaps incrementally moving the collision box closer
        // toward the farthest reachable point.
        Vector2 displacement = maxVelocity * velocity * parameters.dTime;
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
