using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBlender : MonoBehaviour
{


    public Color color1;
    public Color color2;
    public Texture texture;
    public List<Renderer> renderers;

    void Start()
    {
        if (renderers == null)
            renderers = new List<Renderer>();

        if (renderers.Count == 0)
        {
            Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
            renderers.AddRange(childRenderers);
        }

        ApplyMaterialProperties();
    }

    void Update()
    {
        ApplyMaterialProperties();
    }

    void ApplyMaterialProperties()
    {
        Color blendedColor = (color1 + color2) / 2f;
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = blendedColor;
            renderer.material.mainTexture = texture;
        }
    }
}

