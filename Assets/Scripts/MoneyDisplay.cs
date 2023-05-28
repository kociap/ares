using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour
{
    public Text text;

    public int money = 0;

    public void AddMoney(int newMoney)
    {
        Debug.Log("Display");
        money += newMoney;
        text.text = "Money: "+ money.ToString();
    }
}
