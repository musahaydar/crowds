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
}
