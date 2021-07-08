using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Emotion { HAPPY, IDLE, SAD, ANGRY, SCARED }
public enum Destination { NONE, RESTAURANT, THEATRE, LIBRARY }

public class PersonData {
    public Emotion emotionState = Emotion.HAPPY;
    public Destination dest = Destination.NONE;
    public float posX, posY;
    public bool isImposter;
    
    public PersonData(Emotion em, float x, float y) {
        emotionState = em;
        posX = x;
        posY = y;
        dest = Destination.NONE;
        // based on when this override is used
        isImposter = false;
    }

    // override to also set desintation for imposter
    public PersonData(Emotion em, float x, float y, Destination d) {
        emotionState = em;
        posX = x;
        posY = y;
        dest = d;
        // based on when this override is used
        isImposter = true;
    }
}

public class PersonController : MonoBehaviour {
    // set in inspector
    public Sprite sprHappy, sprIdle, sprSad, sprAngry, sprScared;
    public Sprite sprDestRestaurant, sprDestTheatre, sprDestLibrary;
    public Material matDefault, matOutlined;
    public GameObject destinationSprite;
    public SpriteRenderer destinationSpriteRenderer;
    public List<PersonController> peopleInTrigger = new List<PersonController>();

    public bool changedEmotionThisTurn = false;
    public bool finishedTurn = false;

    PersonData data;
    Vector3 movementTarget;
    SpriteRenderer spriteRenderer;
    float turnStepDelay = 0.25f;

    void Start() {
        // why is this not getting called on instantiate??
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        destinationSpriteRenderer = destinationSprite.GetComponent<SpriteRenderer>();
    }

    public void InitPerson(PersonData inData) {
        data = inData;
        // set initial position
        gameObject.transform.position = new Vector3(inData.posX, inData.posY, gameObject.transform.position.z);
        UpdateEmotionSprite();

        // for spawning imposters
        if(inData.dest != Destination.NONE) {
            UpdateDestinationSprite();
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        // track any people that enter this person's trigger
        if(col.gameObject.tag == "Person") {
            PersonController person = col.transform.parent.GetComponent<PersonController>();
            if(!peopleInTrigger.Contains(person)) {
                peopleInTrigger.Add(person);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        // track any people that leave this person's trigger
        if(col.gameObject.tag == "Person") {
            PersonController person = col.transform.parent.GetComponent<PersonController>();
            if(peopleInTrigger.Contains(person)) {
                peopleInTrigger.Remove(person);
            }
        }
    }

    public IEnumerator DoTurn() {
        // make caller wait a second before checking if finished turn is done
        finishedTurn = false;

        // enable outline in shader
        spriteRenderer.sharedMaterial = matOutlined;
        yield return new WaitForSeconds(turnStepDelay);

        // using Vector3.zero as a null value (no target set)
        Vector3 targetPosition = Vector3.zero;

        switch(data.emotionState) {
        case Emotion.HAPPY:
            // a happy person with no destination will choose one
            if(data.dest == Destination.NONE) {
                SetDestination(GetRandomDestination());
                yield return new WaitForSeconds(turnStepDelay);
            }

            // they will update all happy people around them with their destination if they have none
            // only pass the destination on if the person has not yet had their turn
            foreach(PersonController person in peopleInTrigger) {
                if(person.data.emotionState == Emotion.HAPPY && person.data.dest == Destination.NONE && !person.finishedTurn) {
                    person.SetDestination(data.dest);
                    yield return new WaitForSeconds(turnStepDelay);
                }
            }

            // they will make a sad person happy
            foreach(PersonController person in peopleInTrigger) {
                if(person.data.emotionState == Emotion.SAD) {
                    person.UpdateEmotion(Emotion.HAPPY);
                    break;
                }
            }

            // finally, they will move a limited distance towards the destination
            switch(data.dest) {
            case Destination.RESTAURANT:
                targetPosition = GameController.instance.restaurantLocation;
                break;
            case Destination.THEATRE:
                targetPosition = GameController.instance.theatreLocation;
                break;
            case Destination.LIBRARY:
                targetPosition = GameController.instance.libraryLocation;
                break;
            }
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

        // move towards target if a target is set
        if(targetPosition != Vector3.zero) {
            // loop for a certain amount of time, calling move towards
            for(int i = 0; i < 75; i++) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, 0.025f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        // reset destination for a happy person
        ResetDestination();

        // disable outline in shader
        yield return new WaitForSeconds(turnStepDelay);
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

    private void UpdateDestinationSprite() {
        if(destinationSpriteRenderer == null) {
            destinationSpriteRenderer = destinationSprite.GetComponent<SpriteRenderer>();
        }

        switch(data.dest) {
        case Destination.NONE:
            // disable destination prompt sprite
            destinationSprite.SetActive(false);
            break;
        case Destination.RESTAURANT:
            destinationSprite.SetActive(true);
            destinationSpriteRenderer.sprite = sprDestRestaurant;
            break;
        case Destination.THEATRE:
            destinationSprite.SetActive(true);
            destinationSpriteRenderer.sprite = sprDestTheatre;
            break;
        case Destination.LIBRARY:
            destinationSprite.SetActive(true);
            destinationSpriteRenderer.sprite = sprDestLibrary;
            break;
        }
    }

    public void SetDestination(Destination d) {
        data.dest = d;
        // update enable destination target sprite
        UpdateDestinationSprite();
    }

    public void ResetDestination() {
        data.dest = Destination.NONE;
        // disable destination target sprite
        UpdateDestinationSprite();
    }

    public bool IsImposter() {
        return data.isImposter;
    }

    // ideally, the choice of destination should not be random since it makes the solutions to the 
    // puzzle random each time the game is played. this is good enough for the jam version
    private Destination GetRandomDestination() {
        // TODO
        return Destination.LIBRARY;
    }
}
