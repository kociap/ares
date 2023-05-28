using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public AudioClip clip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin");
            GameObject.FindGameObjectWithTag("MoneyDisplay").GetComponent<MoneyDisplay>().AddMoney(1);
            SoundPlayer.PlaySound(clip);
            Destroy(gameObject);
        }
    }
}
