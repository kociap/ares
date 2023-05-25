using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller))]
public class Player: MonoBehaviour {
    private Controller controller;
    private Vector2 movement;

    private void Start() {
        controller = GetComponent<Controller>();
    }

    private void FixedUpdate() {
        Controller.UpdateParameters parameters = new Controller.UpdateParameters{
            movement = movement,
            dTime = Time.fixedDeltaTime,
            time = Time.time
        };
        controller.UpdateController(parameters);
    }

    private void OnMove(InputValue input) {
        movement = input.Get<Vector2>();
        if(movement.sqrMagnitude > 1) {
            movement.Normalize();
        }
    }
}
