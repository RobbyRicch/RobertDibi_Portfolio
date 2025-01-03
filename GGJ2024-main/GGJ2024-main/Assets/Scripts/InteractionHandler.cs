using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    [Header("Input")]
    public KeyCode Channel = KeyCode.Q;



    [Header("Notes")]
    public int claimedNotes = 0;

    [Header("Short Channeling Progress")]
    public float shortChannelProgress = 0;
    public float shortRequiredProgress = 50;
    public Slider shortProgressSlider;
    public float shortFillSpeed = 10f;

    [Header("Long Channeling Progress")]
    public float LongChannelProgress = 0;
    public float LongRequiredProgress = 100;
    public Slider LongProgressSlider;
    public float longFillSpeed = 10f;


}

    
