using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WelcomeManager : MonoBehaviour
{
    public void LoadGame(string team)
    {
        PlayerPrefs.SetString("Team", team);
        SceneManager.LoadScene("DevScene");
    }
}
