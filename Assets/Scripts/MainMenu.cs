using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private Coroutine loadingRoutine;

	// Use this for initialization
	void Start() 
    {
        Application.targetFrameRate = 60;
        AudioManager.Instance.GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	public void GoToGame()
    {
        if (loadingRoutine == null)
            loadingRoutine = StartCoroutine(LoadGameScene(2.0f));
    }

    private IEnumerator LoadGameScene(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        GameManager.CurrentLevel = 0;
        SceneManager.LoadScene("Game");
    }
}
