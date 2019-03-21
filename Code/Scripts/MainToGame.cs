using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainToGame : MonoBehaviour {

    [SerializeField]
    string level = "Level 1";

    public void EnterGameButtonPressed()
    {
        level.Trim();
        Debug.Log("Trying to load Level: " + level);
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
}
