using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pickups
{
    Decelerator,
    MeshaJaws,
    Phase,
    Bat,
    DRS
}
public class PickupAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerData _playerData;
    [Header("Bats References")]
    [SerializeField] private GameObject _LeftBat;
    [SerializeField] private GameObject _RightBat;
    [Header("Grenades References")]
    public GameObject LeftGrenade;
    public GameObject RightGrenade;

    public bool CheckAnimationEnded(Pickups pickup)
    {
        switch (pickup)
        {
            case Pickups.Decelerator:
                /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("GrenadeR") || _animator.GetCurrentAnimatorStateInfo(0).IsName("GrenadeL"))
                {
                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.66f)
                    {
                        return true;
                    }
                }*/
                if (_animator.GetBool("Exited"))
                {
                    return true;
                }
                break;
            case Pickups.MeshaJaws:
                /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Mecha_Jaws_R") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Mecha_Jaws_L"))
                {
                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.82f)
                    {
                        return true;
                    }
                }*/
                if (_animator.GetBool("Exited"))
                {
                    return true;
                }
                break;
            case Pickups.Phase:
                break;
            case Pickups.Bat:
                /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("BatSwing_R2L") || _animator.GetCurrentAnimatorStateInfo(0).IsName("BatSwing_L2R"))
                {
                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.84f)
                    {
                        TurnOffBat();
                        return true;
                    }
                }*/
                if (_animator.GetBool("Exited"))
                {
                    TurnOffBat();
                    return true;
                }
                break;
            case Pickups.DRS:
                //if (_animator.GetCurrentAnimatorStateInfo(0).IsName("DRS_R") || _animator.GetCurrentAnimatorStateInfo(0).IsName("DRS_L"))
                //{
                    if (_animator.GetBool("Exited"))
                    {
                        return true;
                    }
                //}
                break;
        }
        return false;
    }

    IEnumerator WaitToEnableObject(BoxCollider objectToTurnOn,float timeToWait,float timeToTurnOff)
    {
        yield return new WaitForSeconds(timeToWait);
        objectToTurnOn.enabled = true;
        yield return new WaitForSeconds(timeToTurnOff);
        objectToTurnOn.enabled = false;
    }

    public void PlayBatAnim(bool isLeft)
    {
        if (isLeft)
        {
            _animator.SetTrigger("BatLeft");
            _LeftBat.SetActive(true);
            StartCoroutine(WaitToEnableObject(_playerData.ModelData.ItemsOrigin[1].GetComponent<BoxCollider>(), 0.3f,0.4f));
            //_playerData.ItemsOrigin[1].GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            _animator.SetTrigger("BatRight");
            _RightBat.SetActive(true);
            StartCoroutine(WaitToEnableObject(_playerData.ModelData.ItemsOrigin[2].GetComponent<BoxCollider>(), 0.3f,0.4f));
            //_playerData.ItemsOrigin[2].GetComponent<BoxCollider>().enabled = true;
        }
        //Invoke("TurnOffBat", 0.5f);
    }
    private void TurnOffBat()
    {
        _LeftBat.SetActive(false);
        _RightBat.SetActive(false);
        _playerData.ModelData.ItemsOrigin[1].GetComponent<BoxCollider>().enabled = false;
        _playerData.ModelData.ItemsOrigin[2].GetComponent<BoxCollider>().enabled = false;
    }

    public void PlayGranadeAnim(bool isLeft)
    {
        if (isLeft)
        {
            _animator.SetTrigger("DeceleratorLeft");
            LeftGrenade.SetActive(true);
        }
        else
        {
            _animator.SetTrigger("DeceleratorRight");
            RightGrenade.SetActive(true);
        }
        Invoke("TurnOffGranade", 1);
    }
    private void TurnOffGranade()
    {
        LeftGrenade.SetActive(false);
        RightGrenade.SetActive(false);
    }

    public void PlayDRSAnim(bool isLeft)
    {
        if (isLeft)
        {
            _animator.SetTrigger("DRSLeft");
        }
        else
        {
            _animator.SetTrigger("DRSRight");
        }
    }

    public void PlayJawsAnim(bool isLeft)
    {
        if (isLeft)
        {
            _animator.SetTrigger("JawsLeft");
        }
        else
        {
            _animator.SetTrigger("JawsRight");
        }
    }
}
