using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerWorldUI : MonoBehaviour
{
    [SerializeField] private GameObject _playerUI;
    public GameObject PlayerUI => _playerUI;

    [SerializeField] private PlayerInputHandler _player;
    [SerializeField] private Sprite _playerIcon;
    [SerializeField] private RawImage _energyBarFillImg;
    [SerializeField] private Image _pointTipGlow, _readyImg, _pickupImg;

    private Vector2 _maxEnergyBarSize = Vector2.zero;
    private Color _playerColor;

    [Header("Debug")]
    [SerializeField] private bool _isDebugMessagesOn;

    private void Start()
    {
        _maxEnergyBarSize = _energyBarFillImg.rectTransform.sizeDelta;
        _playerColor = _player.SetupData.ColorData.EmissionEmissionColor;
    }

    #region General UI Methods
    public void ChangeEnergyBarLenght(float energyPercentage)
    {
        float energyFraction = energyPercentage / 100f;
        float newHeight = Mathf.Lerp(0, _maxEnergyBarSize.y, energyFraction);
        _energyBarFillImg.rectTransform.sizeDelta = new Vector2(_maxEnergyBarSize.x, newHeight);
    }
    public void ChangeEnergyColor()
    {
        _energyBarFillImg.color = _playerColor;
    }
    public void ChangeEnergyColor(Color color)
    {
        _pointTipGlow.color = color;
        _energyBarFillImg.color = color;
    }

    #endregion

    #region Player Dead or Alive

    #endregion

    #region Player Ready State
    public void HidePlayerReadyImage()
    {
        _readyImg.enabled = false;
    }
    public void ShowPlayerReadyImage()
    {
        _readyImg.enabled = true;
    }
    #endregion

    #region Player Pickups
    public void DiscardPlayerActivePickup()
    {
        _pickupImg.enabled = false;
        //_pickupImg.sprite = _pickupSprites[0];
    }
    public void SetPlayerActivePickup(int pickUpID)
    {
        //_pickupImg.sprite = _pickupSprites[pickUpID];
        _pickupImg.enabled = true;
    }
    #endregion

    #region Player Win Points (currently not in use)

    #endregion

    #region Events
    
    #endregion
}
