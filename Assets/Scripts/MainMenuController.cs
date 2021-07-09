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
        SoundBankController.instance.buttonPress.Play();
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(scene);
        yield return null;
    }

    public void QuitGame() {
        StartCoroutine(QuitGameHelper());
    }


    public IEnumerator QuitGameHelper() {
        // add delay for sound effect
        SoundBankController.instance.buttonPress.Play();
        yield return new WaitForSeconds(0.1f);
        Application.Quit();
        yield return null;
    }
}
