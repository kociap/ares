using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atack : MonoBehaviour
{

    [SerializeField]
    private int damageToImpose;
    [SerializeField]
    private string toAttackTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collider " + other + "; tag " + toAttackTag);
        if (other.CompareTag(toAttackTag))
        {
            var hp = other.GetComponent<Hp1>();
            hp.dealDamage(damageToImpose);
        }
    }
}
