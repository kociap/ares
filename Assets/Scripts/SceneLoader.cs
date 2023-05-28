using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void startLevel()
    {
        SceneManager.LoadScene("Scenes/WorldGen");
    }

    public void loadMenu()
    {
        SceneManager.LoadScene("Scenes/Menu");
    }

    public void loadDeathScreen()
    {
        SceneManager.LoadScene("Scenes/Lose");
    }
}
