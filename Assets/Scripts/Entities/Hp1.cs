using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp1 : MonoBehaviour
{

    public int initialHp;

    private int hpLevel;

    // Start is called before the first frame update
    void Start()
    {
        hpLevel = initialHp;
    }

    public void dealDamage(int damage){
        hpLevel -= damage;
        if(hpLevel <= 0)
        {
            Destroy(gameObject);
        }
    }
}
