using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    
    public void LoadScene(string scene) {
        StartCoroutine(LoadSceneHelper(scene));
    }

    public IEnumerator LoadSceneHelper(string scene) {
        // add delay for sound effect
        SceneManager.LoadScene(scene);
        yield return null;
    }

    public void QuitGame() {
        Application.Quit();
    }
}
