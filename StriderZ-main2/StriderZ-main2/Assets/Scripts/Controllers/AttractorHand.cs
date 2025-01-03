using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorHand : MonoBehaviour
{
    [SerializeField] private bool _isRightHand = false;
    [SerializeField] private AttractorController _attractorController;
    public void CancelAttractor()
    {
        if (_isRightHand)
            _attractorController.CancelAttractorRight(true);
        else
            _attractorController.CancelAttractorLeft(true);
    }
}
