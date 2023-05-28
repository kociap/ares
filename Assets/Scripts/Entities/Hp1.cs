using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Hp1 : MonoBehaviour
{
    public bool isPlayer = false;

    public int initialHp;

    public AudioClip dieSound;

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
            if (!isPlayer)
            {
                if(dieSound != null){
                    SoundPlayer.PlaySound(dieSound);
                }
                Destroy(gameObject);
            }
            else
            {
                SceneManager.LoadScene("Scenes/Lose");
            }
        }
    }
}
