using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuShuffler : MonoBehaviour
{
    public List<GameObject> stridersIdles;
    public List<Sprite> backgroundImages;
    public List<GameObject> platformPrefabs;
    public List<Sprite> selectionBackground;
    public List<GameObject> podiumPrefabs; // New list for podium prefabs
    public Image backgroundHolder;
    public Image selectionBackgroundHolder;

    private void Start()
    {
        // Instantiate a random GameObject from the list
        int randomIndex = Random.Range(0, stridersIdles.Count);
        GameObject instantiatedObject = Instantiate(stridersIdles[randomIndex]);

        // Set a reference to a random Image from the list for the background
        randomIndex = Random.Range(0, backgroundImages.Count);
        Sprite selectedBackgroundImage = backgroundImages[randomIndex];

        backgroundHolder.sprite = selectedBackgroundImage;

        // Set a reference to a corresponding selection background Image
        if (randomIndex < selectionBackground.Count)
        {
            Sprite selectedSelectionBackground = selectionBackground[randomIndex];
            selectionBackgroundHolder.sprite = selectedSelectionBackground;
        }
        else
        {
            Debug.LogWarning("No corresponding selection background image for the selected background image.");
        }

        // Spawn a platform prefab according to the same index as the background image
        if (randomIndex < platformPrefabs.Count)
        {
            GameObject platformPrefab = platformPrefabs[randomIndex];
            Instantiate(platformPrefab);
        }
        else
        {
            Debug.LogWarning("No corresponding platform prefab for the selected background image.");
        }

        // Spawn a podium prefab according to the same index as the background image
        if (randomIndex < podiumPrefabs.Count)
        {
            GameObject podiumPrefab = podiumPrefabs[randomIndex];
            Instantiate(podiumPrefab);
        }
        else
        {
            Debug.LogWarning("No corresponding podium prefab for the selected background image.");
        }
    }
}
