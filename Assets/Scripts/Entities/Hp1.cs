using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Hp1 : MonoBehaviour
{
    public bool isPlayer = false;

    public int initialHp;

    public AudioClip dieSound;
    public AudioClip ouchSound;

    public GameObject drop;

    private int hpLevel;
    

    // Start is called before the first frame update
    void Start()
    {
        hpLevel = initialHp;
    }

    public void dealDamage(int damage){
        hpLevel -= damage;
        if(isPlayer)
        {
            SoundPlayer.PlaySound(ouchSound);
        }
        if(hpLevel <= 0)
        {
            if (!isPlayer)
            {
                if(dieSound != null){
                    SoundPlayer.PlaySound(dieSound);
                }
                if(drop != null)
                {
                    var newDrop = Instantiate(drop);
                    newDrop.transform.position = transform.position;
                }
                Destroy(gameObject);
            }
            else
            {
                PlayerPrefs.SetInt("Money", 0);
                SceneManager.LoadScene("Scenes/Lose");
            }
        }
    }
}
