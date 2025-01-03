using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorWheel : MonoBehaviour
{

    public bool _isplayer1;
    //public MeshRenderer _player; 


    [Header("Property")]
    public Image _preview;
   
    [Header("Sliders")]

    public Slider _rSlider;
    public Slider _gSlider;
    public Slider _bSlider;

    

     void Update()
    {
        Color _targetColor = new Color(_rSlider.value, _gSlider.value, _bSlider.value);
        _preview.color = _targetColor;
        //_player.material.color = _targetColor;

        if (_isplayer1)
        {
            SceneHelper.AllPlayerColors[0] = _targetColor;
        }
        else
        {
            SceneHelper.AllPlayerColors[1] = _targetColor;
        }
    }
}
