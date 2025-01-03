using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeBuilderManager : MonoBehaviour
{
    [Header("Colors")]
    public List<Color> colors;
    private int currentColorIndex = 0;

    [Header("Shapes")]
    public List<GameObject> shapes;
    private int currentShapeIndex = 0;

    [Header("UI Elements")]
    public Button nextColorButton;
    public Button prevColorButton;
    public Button nextShapeButton;
    public Button prevShapeButton;
    public Button finalizeButton;

    private GameObject currentSlime;
    private Renderer currentRenderer;

    void Start()
    {
        if (colors == null || colors.Count == 0)
            colors = new List<Color> { Color.white };

        if (shapes == null || shapes.Count == 0)
            Debug.LogError("No shapes assigned!");

        nextColorButton.onClick.AddListener(NextColor);
        prevColorButton.onClick.AddListener(PrevColor);
        nextShapeButton.onClick.AddListener(NextShape);
        prevShapeButton.onClick.AddListener(PrevShape);
        finalizeButton.onClick.AddListener(FinalizeSlime);

        InstantiateCurrentSlime();
    }

    void InstantiateCurrentSlime()
    {
        if (currentSlime != null)
            Destroy(currentSlime);

        currentSlime = Instantiate(shapes[currentShapeIndex], transform.position, Quaternion.identity);
        currentSlime.SetActive(true); // Ensure the instantiated GameObject is active
        currentRenderer = currentSlime.GetComponent<Renderer>();
        ApplyColor();
    }

    void ApplyColor()
    {
        if (currentRenderer != null)
        {
            currentRenderer.material.color = colors[currentColorIndex];
        }
    }

    void NextColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Count;
        ApplyColor();
    }

    void PrevColor()
    {
        currentColorIndex = (currentColorIndex - 1 + colors.Count) % colors.Count;
        ApplyColor();
    }

    void NextShape()
    {
        currentShapeIndex = (currentShapeIndex + 1) % shapes.Count;
        InstantiateCurrentSlime();
    }

    void PrevShape()
    {
        currentShapeIndex = (currentShapeIndex - 1 + shapes.Count) % shapes.Count;
        InstantiateCurrentSlime();
    }

    void FinalizeSlime()
    {
        // Display the final product or save it as needed
        Debug.Log("Finalized Slime with color: " + colors[currentColorIndex] + " and shape: " + shapes[currentShapeIndex].name);
    }
}