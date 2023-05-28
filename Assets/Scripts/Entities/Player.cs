using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller))]
public class Player: Entitiy
{
    [SerializeField]
    private Controller controller;
    [SerializeField]
    private Gun gun;

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
        Debug.Log("Movement!");
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
