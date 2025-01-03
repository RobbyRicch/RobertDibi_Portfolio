using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColor : MonoBehaviour
{
    public Color color; // The color of the vial
    
    void Start()
    {
        gameObject.GetComponent<RawImage>().color = color;

    }

}
