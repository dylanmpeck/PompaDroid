using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour {

    public void GameOverDone()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
