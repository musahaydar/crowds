using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBankController : MonoBehaviour {
    public static SoundBankController instance;

    // audio sources are children of the soundbank controller
    public AudioSource pieceAppear1, pieceAppear2, pieceAppear3;
    public AudioSource beginTurn;
    public AudioSource buttonHover;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this) {
            Destroy(this.gameObject);
        }
    }

    public void PlayPieceAppear() {
        int r = Random.Range(0,3);
        switch(r) {
        case 0:
            pieceAppear1.Play();
            break;
        case 1:
            pieceAppear2.Play();
            break;
        case 2:
            pieceAppear3.Play();
            break;
        }
    }
}
