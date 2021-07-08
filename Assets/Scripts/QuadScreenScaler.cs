using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScreenScaler : MonoBehaviour
{
    void Start()
    {
        float quadHeight = Camera.main.orthographicSize * 2.2f;
        float quadWidth = quadHeight * Screen.width / Screen.height;
        transform.localScale = new Vector3(quadWidth, quadHeight, 1);   
    }
}
