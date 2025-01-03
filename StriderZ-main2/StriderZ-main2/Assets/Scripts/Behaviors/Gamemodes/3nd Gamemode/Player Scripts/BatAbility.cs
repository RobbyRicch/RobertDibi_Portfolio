using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAbility : Weapon
{
    //[SerializeField] private BoxCollider BatBoxColRef;
    [SerializeField] private Animation anim;

    public override void UseWeapon()
    {
        //BatRef.SetActive(true);
        anim.Play();
    }
}
