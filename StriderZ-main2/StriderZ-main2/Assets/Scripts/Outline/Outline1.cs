using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline1 : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private float _outlineScaleFactor;
    [SerializeField] private Color _outlineColor;
    private Renderer _outlineRenderer;

    private void Start()
    {
        _outlineRenderer = CreateOutline(_outlineMaterial, _outlineScaleFactor, _outlineColor);
        _outlineRenderer.enabled = true;
    }

    private Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {
        GameObject outlineObject = Instantiate(gameObject, transform.position, transform.rotation, transform);
        Renderer rend = outlineObject.GetComponent<Renderer>();
        
        rend.material = outlineMat;
        rend.material.SetColor("_OutlineColor", color);
        rend.material.SetFloat("_Scale", scaleFactor);
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        outlineObject.GetComponent<Outline1>().enabled = false;
        outlineObject.GetComponent<Collider>().enabled = false;

        rend.enabled = false;

        return rend;
    }
}

