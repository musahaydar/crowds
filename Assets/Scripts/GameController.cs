using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public GameObject personPrefab;
    public GameObject buildingPrefab;
    public GameObject imposterPrefab;
    public Button turnStartButton;
    // public bool readyToMove = false;
    public float readyDelay = 0.5f;
    public float loadPersonDelay = 0.3f;

    // used as targets for PersonController
    public Vector3 restaurantLocation, theatreLocation, libraryLocation;
    public List<Vector3> houseLocations = new List<Vector3>();

    // data structs for creating levels
    // jam version: call LoadJamPuzzle() to fill structs with handmade jam level
    // call GenerateGame() to fill these structs with generated data
    List<PersonData> people = new List<PersonData>();
    List<BuildingData> buildings = new List<BuildingData>();

    // data structs for gameplay loop
    // queue of imposters
    List<PersonController> imposterQueue = new List<PersonController>();
    List<PersonController> peopleQueue = new List<PersonController>();
    List<PersonController> personRemovalQueue = new List<PersonController>();

    IEnumerator setImposterInstance = null;
    GameObject imposterInHand;

    // these should probably not be constants
    float boardMinX = -0.2f;
    float boardMaxX = 8.2f;
    float boardMinY = -8.05f;
    float boardMaxY = 0.5f;

    // display these for the player
    int impostersPlayed = 0;
    int turnsPlayed = 0;
    
    void Awake() {
        if(instance == null) {
            instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this) {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        turnStartButton.interactable = false;
        // temp: for testing
        // (actually this might be good enough for the jam build)
        GenerateGame();
    }

    public IEnumerator LoadGame() {
        // load buildings (instant)
        foreach(BuildingData buildingData in buildings) {
            GameObject buildingObj = Instantiate(buildingPrefab);
            buildingObj.GetComponent<BuildingController>().InitBuilding(buildingData);

            // store building location
            BuildingType buildingType = buildingObj.GetComponent<BuildingController>().data.buildingType;
            switch(buildingType) {
            case BuildingType.HOUSE:
                houseLocations.Add(buildingObj.transform.position);
                break;
            case BuildingType.RESTAURANT:
                restaurantLocation = buildingObj.transform.position;
                break;
            case BuildingType.THEATRE:
                theatreLocation = buildingObj.transform.position;
                break;
            case BuildingType.LIBRARY:
                libraryLocation = buildingObj.transform.position;
                break;
            }
        }
        yield return new WaitForSeconds(loadPersonDelay);
        
        // load people (with some delay)
        foreach(PersonData personData in people) {
            GameObject personObj = Instantiate(personPrefab);
            personObj.GetComponent<PersonController>().InitPerson(personData);
            // add to people queue for gameplay loop
            peopleQueue.Add(personObj.GetComponent<PersonController>());
            // TODO: add sound/visual effect
            yield return new WaitForSeconds(loadPersonDelay);
        }
        
        // start coroutine that reads mouse position for person turn number
        StartCoroutine(checkMousePosition());

        // now waiting for player input button to start the round
        yield return new WaitForSeconds(loadPersonDelay);
        turnStartButton.interactable = true;
        yield return null;
    }

    // called from pressing button unity event
    public void StartTurn() {        
        StartCoroutine(DoTurn());
    }

    public IEnumerator DoTurn() {
        // disable the button
        turnStartButton.interactable = false;
        turnsPlayed++;

        // destory any imposter in hand
        if(setImposterInstance != null) {
            Destroy(imposterInHand);
            StopCoroutine(setImposterInstance);
        }
        
        // have each imposter take their turn
        foreach(PersonController imposter in imposterQueue) {
            StartCoroutine(imposter.DoTurn());
            // delay before checking if the value is set
            yield return new WaitForSeconds(readyDelay);
            while(!imposter.finishedTurn) {
                yield return null;
            }
        }

        // have each person take their turn
        foreach(PersonController person in peopleQueue) {
            StartCoroutine(person.DoTurn());
            // delay before checking if the value is set
            yield return new WaitForSeconds(readyDelay);
            while(!person.finishedTurn) {
                yield return null;
            }
        }

        // now allow all the people to move on the board
        // readyToMove = true;

        // move all imposters to front of people queue so new imposters go first
        for(int i = imposterQueue.Count - 1; i >= 0; i--) {
            peopleQueue.Insert(0, imposterQueue[i]);
        }
        imposterQueue.Clear();

        // remove all people in the removal queue
        foreach(PersonController person in personRemovalQueue) {
            if(peopleQueue.Contains(person)) {
                peopleQueue.Remove(person);
            }
            Destroy(person.gameObject);
        }
        personRemovalQueue.Clear();
        // check win condition here, since this is the only place a person is removed from the game

        // prepare all people for next turn
        foreach(PersonController person in peopleQueue) {
            person.finishedTurn = false;
            person.changedEmotionThisTurn = false;
        }

        yield return new WaitForSeconds(0.5f);
        // turn over, enable the start turn button and wait for user input event
        turnStartButton.interactable = true;
        yield return null;
    }

    public void SetImposterButton(int whichImposter) {
        // if the player has started the turn, do nothing
        if(turnStartButton.interactable == false) {
            return;
        }

        // if the player is currently setting an imposter, delete it
        if(setImposterInstance != null) {
            Destroy(imposterInHand);
            StopCoroutine(setImposterInstance);
        }

        // start the setting imposter coroutine again
        setImposterInstance = SetImposter(whichImposter);
        StartCoroutine(setImposterInstance);
    }

    private IEnumerator SetImposter(int whichImposter) {
        // set te imposter emotion
        // also set imposter saying for cases 0-2
        Emotion imposterEmotion = Emotion.HAPPY;
        Destination imposterDest = Destination.NONE;
        switch(whichImposter) {
        case 0:
            // imposter says resturant
            imposterDest = Destination.RESTAURANT;
            break;
        case 1:
            // imposter says theatre
            imposterDest = Destination.THEATRE;
            break;
        case 2:
            // imposter says library
            imposterDest = Destination.LIBRARY;
            break;
        case 4:
            imposterEmotion = Emotion.SAD;
            break;
        case 5:
            imposterEmotion = Emotion.ANGRY;
            break;
        case 6:
            imposterEmotion = Emotion.IDLE;
            break;
        case 7:
            imposterEmotion = Emotion.SCARED;
            break;
        }

        // create an imposter gameobject
        imposterInHand = Instantiate(imposterPrefab);
        imposterInHand.GetComponent<PersonController>().InitPerson(new PersonData(imposterEmotion, -10, -10, imposterDest));
        // make the imposter appear transparent
        imposterInHand.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        // this is probably inefficient
        if(imposterDest != Destination.NONE) {
            imposterInHand.GetComponent<PersonController>().destinationSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }

        // loop until player clicks left mouse button
        while(true) {
            // move the new imposter to the cursor position
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            imposterInHand.transform.position = new Vector3(target.x, target.y, gameObject.transform.position.z);
            if(Input.GetMouseButtonDown(0)) {
                // check if the cursor is in a valid position on the board
                // set it on the board
                if(target.x <= boardMaxX && target.x >= boardMinX && target.y <= boardMaxY && target.y >= boardMinY) {
                    // reset transparency in color
                    imposterInHand.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    if(imposterDest != Destination.NONE) {
                        imposterInHand.GetComponent<PersonController>().destinationSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    }
                    // add to imposter queue
                    imposterQueue.Add(imposterInHand.GetComponent<PersonController>());
                    impostersPlayed++;
                    // leave the object on the board
                    imposterInHand = null;
                }
                // remove the imposter in hand if click outside board
                else {
                    Destroy(imposterInHand);
                }
                break;
            }
            // yield return null to prevent infinite loop crash
            yield return null;
        }
    }

    public IEnumerator checkMousePosition() {
        while(true) {
            // skip updating if player is setting an imposter
            if(imposterInHand != null) {
                // temp output
                // Debug.Log("N/A");
                
                yield return null;
                continue;
            }

            // raycast at mouse position to see if there is a person there
            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Collider2D[] col = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));            

            bool foundHit = false;
            foreach(Collider2D hit in col) {
                if(hit && hit.gameObject.tag == "Person") {
                    foundHit = true;
                    PersonController person = hit.transform.parent.GetComponent<PersonController>();
                    int turnNum = 0;

                    if(person.IsImposter()) {
                        turnNum = imposterQueue.IndexOf(person) + 1;
                    } 
                    else {
                        turnNum = imposterQueue.Count;
                        turnNum += peopleQueue.IndexOf(person) + 1;
                    }

                    // temp output
                    // Debug.Log(turnNum);

                    break; // foreach loop
                } 
            }
            if(!foundHit) {
                // temp output
                // Debug.Log("N/A");
            }
            yield return null;
        }
    }

    public void RemovePersonFromBoard(PersonController person) {
        personRemovalQueue.Add(person);
    }
    
    public void GenerateGame() {
        // generate data for data structures
        // vector 2 list to make sure buildings have unique locations
        List<Vector2> locationsUsed = new List<Vector2>();

        // generate location of the three buildings
        Vector2 loc = generateBuildingLocation(locationsUsed);
        buildings.Add(new BuildingData(BuildingType.RESTAURANT, (int)loc.x, (int)loc.y));
        // oh no copied and pasted code gross
        loc = generateBuildingLocation(locationsUsed);
        buildings.Add(new BuildingData(BuildingType.THEATRE, (int)loc.x, (int)loc.y));
        loc = generateBuildingLocation(locationsUsed);
        buildings.Add(new BuildingData(BuildingType.LIBRARY, (int)loc.x, (int)loc.y));

        // generate 1-4 houses
        int numHouses = Random.Range(1, 5);
        for(int i = 0; i < numHouses; i++) {
            loc = generateBuildingLocation(locationsUsed);
            buildings.Add(new BuildingData(BuildingType.HOUSE, (int)loc.x, (int)loc.y));
        }

        // generate people
        int minPeople = 25, maxPeople = 30;
        int numPeople = Random.Range(minPeople, maxPeople);
        for(int i = 0; i < numPeople; i++) {
            Emotion e = generateRandomEmotion();
            float x = Random.Range(boardMinX, boardMaxX);
            float y = Random.Range(boardMinY, boardMaxY);
            people.Add(new PersonData(e, x, y));
        }
        
        // load game
        StartCoroutine(LoadGame());
    }

    private bool isLocationUsed(List<Vector2> locationsUsed, Vector2 location){
        foreach(Vector2 loc in locationsUsed) {
            if(loc == location) {
                return true;
            }
        }
        return false;
    }

    private Vector2 generateBuildingLocation(List<Vector2> locationsUsed) {
        // this loop has potential for infinite loop
        while(true) {
            int x = Random.Range(0, 8);
            int y = Random.Range(-7, 0);
            Vector2 loc = new Vector2(x,y);
            if(!isLocationUsed(locationsUsed, loc)) {
                locationsUsed.Add(loc);
                return loc;
            }
        }
    }

    private Emotion generateRandomEmotion() {
        int r = Random.Range(0,5);
        switch(r) {
        case 0:
            return Emotion.HAPPY;
        case 1:
            return Emotion.SAD;
        case 2:
            return Emotion.ANGRY;
        case 3:
            return Emotion.SCARED;
        case 4:
            return Emotion.IDLE;
        }
        return Emotion.HAPPY;
    }
    
    /* public void LoadJamPuzzle() {
        // fill data structures with preset data
        // add people
        people.Add(new PersonData(Emotion.HAPPY, 2.5f, -0.5f));
        people.Add(new PersonData(Emotion.HAPPY, 3f, -2f));
        // add buildings
        buildings.Add(new BuildingData(BuildingType.RESTAURANT, 6, -1));
        buildings.Add(new BuildingData(BuildingType.HOUSE, 4, -2));
        buildings.Add(new BuildingData(BuildingType.HOUSE, 5, -7));
        buildings.Add(new BuildingData(BuildingType.THEATRE, 2, 0));
        buildings.Add(new BuildingData(BuildingType.LIBRARY, 1, -5));
        // load game
        StartCoroutine(LoadGame());
    } */
}
