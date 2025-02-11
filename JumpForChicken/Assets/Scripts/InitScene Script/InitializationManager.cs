using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationManager : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("Init", 0);
        SceneManager.LoadScene("MainTitle");
    }
}
