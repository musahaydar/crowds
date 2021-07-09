using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {
    static MusicController instance;
    //public string lastLevel = "";
    public bool persistant;

    AudioSource source;

    void Awake() {
        if(persistant) {
            if(instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(instance != this) {
                Destroy(this.gameObject);
            }
        }
    }

    void Start() {
        source = GetComponent<AudioSource>();
    }

    public IEnumerator FadeOutMusic() {
        while(source.volume > 0) {
            source.volume -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        source.Stop();
        source.volume = 0.4f;
        yield return null;
    }

    public void StartMusic() {
        source.Play();
    }
}
