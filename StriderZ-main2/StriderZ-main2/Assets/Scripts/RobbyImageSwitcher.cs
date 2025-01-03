using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobbyImageSwitcher : MonoBehaviour
{
    public List<Texture> imageList;
    public RawImage imageDisplay;

    private int currentIndex = 0;

    private void Start()
    {
        ShowCurrentImage();
    }

    public void NextImage()
    {
        currentIndex = (currentIndex + 1) % imageList.Count;
        ShowCurrentImage();
    }

    public void PreviousImage()
    {
        currentIndex = (currentIndex - 1 + imageList.Count) % imageList.Count;
        ShowCurrentImage();
    }

    private void ShowCurrentImage()
    {
        if (imageList.Count > 0)
            imageDisplay.texture = imageList[currentIndex];
    }
}
