using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBankController : MonoBehaviour {
    public static SoundBankController instance;

    // audio sources are children of the soundbank controller
    public AudioSource pieceAppear1, pieceAppear2, pieceAppear3;
    public AudioSource beginTurn;
    public AudioSource buttonHover;
    public AudioSource angrySelect, angryDrop;
    public AudioSource idleSelect, idleDrop;
    public AudioSource scaredSelect, scaredDrop;
    public AudioSource happySelect, happyDrop;
    public AudioSource sadSelect, sadDrop;
    public AudioSource restaurantSelect, restaurantDrop;
    public AudioSource theatreSelect, theatreDrop;
    public AudioSource librarySelect, libraryDrop;
    public AudioSource walking;
    public AudioSource door;
    public AudioSource changeHappy, changeSad, changeIdle, changeAngry, changeScared;
    public AudioSource pieceFlash1, pieceFlash2, pieceFlash3;
    public AudioSource buttonPress;

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

    public void PlayPieceFlash() {
        int r = Random.Range(0,3);
        switch(r) {
        case 0:
            pieceFlash1.Play();
            break;
        case 1:
            pieceFlash2.Play();
            break;
        case 2:
            pieceFlash3.Play();
            break;
        }
    }

    public void PlayPieceSelect(Emotion em, Destination d) {
        if(d != Destination.NONE) {
            switch(d) {
            case Destination.RESTAURANT:
                restaurantSelect.Play();
                break;
            case Destination.THEATRE:
                theatreSelect.Play();
                break;
            case Destination.LIBRARY:
                librarySelect.Play();
                break;
            }
            return;
        }

        switch(em) {
        case Emotion.HAPPY:
            happySelect.Play();
            break;
        case Emotion.SAD:
            sadSelect.Play();
            break;
        case Emotion.IDLE:
            idleSelect.Play();
            break;
        case Emotion.ANGRY:
            angrySelect.Play();
            break;
        case Emotion.SCARED:
            scaredSelect.Play();
            break;
        }
    }

    public void PlayPieceDrop(Emotion em, Destination d) {
        if(d != Destination.NONE) {
            switch(d) {
            case Destination.RESTAURANT:
                restaurantDrop.Play();
                break;
            case Destination.THEATRE:
                theatreDrop.Play();
                break;
            case Destination.LIBRARY:
                libraryDrop.Play();
                break;
            }
            return;
        }

        switch(em) {
        case Emotion.HAPPY:
            happyDrop.Play();
            break;
        case Emotion.SAD:
            sadDrop.Play();
            break;
        case Emotion.IDLE:
            idleDrop.Play();
            break;
        case Emotion.ANGRY:
            angryDrop.Play();
            break;
        case Emotion.SCARED:
            scaredDrop.Play();
            break;
        }
    }

    public void PlayChangeEmotion(Emotion em) {
        switch(em) {
        case Emotion.HAPPY:
            changeHappy.Play();
            break;
        case Emotion.SAD:
            changeSad.Play();
            break;
        case Emotion.IDLE:
            changeIdle.Play();
            break;
        case Emotion.ANGRY:
            changeAngry.Play();
            break;
        case Emotion.SCARED:
            changeScared.Play();
            break;
        }
    }
}
