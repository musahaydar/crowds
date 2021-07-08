using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType { HOUSE, RESTAURANT, THEATRE, LIBRARY }

public class BuildingData {
    public BuildingType buildingType = BuildingType.HOUSE;
    public int posX, posY;

    public BuildingData(BuildingType b, int x, int y) {
        buildingType = b;
        posX = x;
        posY = y;
    }
}

public class BuildingController : MonoBehaviour {
    public const float yOffset = 0.2f;

    // since it's not important which building a person is in, we can simply track which people are in any building
    public static List<PersonController> peopleInBuildings = new List<PersonController>();

    public BuildingData data;
    
    SpriteRenderer spriteRenderer;

    // set in inspector
    public Sprite sprHouse, sprRestaurant, sprTheatre, sprLibrary;

    public void InitBuilding(BuildingData inData) {
        data = inData;
        // set position based on input
        gameObject.transform.position = new Vector3((float)inData.posX, (float)(inData.posY) + yOffset, gameObject.transform.position.z);
        // set sprite
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        switch(inData.buildingType) {
        case BuildingType.HOUSE:
            spriteRenderer.sprite = sprHouse;
            break;
        case BuildingType.LIBRARY:
            spriteRenderer.sprite = sprLibrary;
            break;
        case BuildingType.THEATRE:
            spriteRenderer.sprite = sprTheatre;
            break;
        case BuildingType.RESTAURANT:
            spriteRenderer.sprite = sprRestaurant;
            break;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Person") {
            PersonController p = col.transform.parent.GetComponent<PersonController>();
            // it is possible for people colliders to be in two buildings at once, so check
            if(!peopleInBuildings.Contains(p)) {
                peopleInBuildings.Add(p);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Person") {
            PersonController p = col.transform.parent.GetComponent<PersonController>();
            // it is possible for people colliders to be in two buildings at once, so check
            if(peopleInBuildings.Contains(p)) {
                peopleInBuildings.Remove(p);
            }
        }
    }
}
