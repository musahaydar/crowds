using UnityEngine;
using System.Collections;
using UnityEngine.UI;
      
//Script sets font filtering to point
//First, set the font's Rendering Mode to "Hinted Raster"
//Add this script to UI Text object (after choosing font) for sharp text (point filtering)
[ExecuteInEditMode]
public class FontFilter : MonoBehaviour {
	void Start () {
		GetComponent<Text> ().font.material.mainTexture.filterMode = FilterMode.Point;
	}
}

