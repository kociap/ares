using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour
{
    public Text text;

    public int money;

    public void Start()
    {
        money = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetInt("Money") : 0;
        text.text = "Money: " + money.ToString();
    }

    public void AddMoney(int newMoney)
    {
        Debug.Log("Display");
        money += newMoney;
        text.text = "Money: "+ money.ToString();
        PlayerPrefs.SetInt("Money", money);
    }
}
