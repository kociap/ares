using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public int colorId = 0;
    public GameObject red;
    public GameObject green;
    public GameObject blue;
    
    public void SetColor(int colorId)
    {
        switch (colorId)
        {
            case 0:
                red.SetActive(true);
                green.SetActive(false);
                blue.SetActive(false);
                this.colorId = 0;
                break;
            case 1:
                red.SetActive(false);
                green.SetActive(true);
                blue.SetActive(false);
                this.colorId = 1;
                break;
            default:
                red.SetActive(false);
                green.SetActive(false);
                blue.SetActive(true);
                this.colorId = 2;
                break;
        }
    }
}
