﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Emotion { HAPPY, IDLE, SAD, ANGRY, SCARED }
public enum Destination { NONE, RESTAURANT, THEATRE, LIBRARY }

public class PersonData {
    public Emotion emotionState = Emotion.HAPPY;
    public float posX, posY;
    
    public PersonData(Emotion em, float x, float y) {
        emotionState = em;
        posX = x;
        posY = y;
    }
}

public class PersonController : MonoBehaviour {
    // set in inspector
    public Sprite sprHappy, sprIdle, sprSad, sprAngry, sprScared;
    public Material matDefault, matOutlined;

    public bool changedEmotionThisTurn = false;
    public bool finishedTurn = false;
    public Destination dest = Destination.NONE;

    PersonData data;
    Vector3 movementTarget;
    SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void InitPerson(PersonData inData) {
        data = inData;
        // set initial position
        gameObject.transform.position = new Vector3(inData.posX, inData.posY, gameObject.transform.position.z);
        UpdateEmotionSprite();
    }

    public IEnumerator DoTurn() {
        // make caller wait a second before checking if finished turn is done
        finishedTurn = false;

        // enable outline in shader
        spriteRenderer.sharedMaterial = matOutlined;
        yield return new WaitForSeconds(0.1f);

        switch(data.emotionState) {
        case Emotion.HAPPY:
            // a happy person with no destination will choose one
            // they will update all happy people around them with their destination if they have none
            // they will make a sad person happy
            // finally, they will move a limited distance towards the destination
            break;
        case Emotion.IDLE:
            // and idle person will become happy
            UpdateEmotion(Emotion.HAPPY);
            break;
        case Emotion.SAD:
            // a sad person will make one person around them sad, and stay in place
            break;
        case Emotion.ANGRY:
            // an angry person will make one person around them angry
            // and then move in the opposite direction of that person
            break;
        case Emotion.SCARED:
            // a scared person will move towards the nearest building
            break;
        }

        // do any movements (in turn, not all at once)

        // reset destination for a happy person
        ResetDestination();

        // disable outline in shader
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sharedMaterial = matDefault;

        finishedTurn = true;
        yield return null;
    }

    /* public IEnumerator Move() {
        // wait until all people are ready to move
        while(!GameController.instance.readyToMove) {
            yield return new WaitForSeconds(GameController.instance.loopDelay);
        }
        
        // handle any movements this person needs to make

        yield return null;
    } */

    private void UpdateEmotionSprite() {
        if(spriteRenderer == null) {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        switch(data.emotionState) {
        case Emotion.HAPPY:
            spriteRenderer.sprite = sprHappy;
            break;
        case Emotion.IDLE:
            spriteRenderer.sprite = sprIdle;
            break;
        case Emotion.SAD:
            spriteRenderer.sprite = sprSad;
            break;
        case Emotion.ANGRY:
            spriteRenderer.sprite = sprAngry;
            break;
        case Emotion.SCARED:
            spriteRenderer.sprite = sprScared;
            break;
        }
    }

    public void UpdateEmotion(Emotion e) {
        data.emotionState = e;
        UpdateEmotionSprite();
    }

    public void SetDestination(Destination d) {
        dest = d;
        // update enable destination target sprite
    }

    public void ResetDestination() {
        dest = Destination.NONE;
        // disable destination target sprite
    }
}
