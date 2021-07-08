using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsoluteSortingOrder : MonoBehaviour {

    public float offset;

    //sets an objects sorting order relative to its Y position
    void Update() {
        GetComponent<SpriteRenderer>().sortingOrder = (Mathf.RoundToInt((transform.position.y + offset) * 100f) / 4) * -1;
    }
}
