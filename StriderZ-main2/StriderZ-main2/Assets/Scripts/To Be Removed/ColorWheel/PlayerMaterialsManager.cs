using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialsManager : MonoBehaviour
{
    [SerializeField] private List<Renderer> m_Renderer;
    [SerializeField] private List<Material> materials;
    [SerializeField] private float Height = 5f;
    private void Start()
    {
        foreach (var ren in m_Renderer)
        {
            foreach (var mat in ren.materials)
            {
                materials.Add(mat);
            }
        }
    }
    private void Update()
    {
        DissolveEffect();
    }

    private void DissolveEffect()
    {
        foreach (var mat in materials)
        {
            mat.SetFloat("_Cutoff_Height", Height);
        }
    }
}
