using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorIsLavaEffect : MonoBehaviour
{
    public Renderer renderer;
    public AnimationCurve curve;
    private float time = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        renderer.material.SetFloat("_Sphere_Mask_Radius", curve.Evaluate(time)); 
        time += Time.deltaTime;
    }
}
