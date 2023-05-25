using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atack : MonoBehaviour
{

    public int damageToImpose;

    public string toAtackTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(toAtackTag))
        {
            var hp = other.GetComponent<Hp1>();
            hp.dealDamage(damageToImpose);
        }
    }
}
