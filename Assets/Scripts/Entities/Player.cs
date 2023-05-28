using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller))]
public class Player: Entitiy
{
    public Gun gun;
    private Controller controller;

    private void Start() {
        controller = GetComponent<Controller>();
    }

    private void Update() {
        Controller.UpdateParameters parameters = new Controller.UpdateParameters{
            movement = movement,
            dTime = Time.deltaTime,
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

    private void OnAttack() {
        Debug.Log("Attack!");
        gun.tryToShoot();
    }

    private void OnDash() {
        Debug.Log("Dash!");
        controller.BeginDash();
    }
}
