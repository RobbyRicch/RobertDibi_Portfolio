using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunSideArm : GunBase
{
    protected override void Load(Profile data)
    {
        /*if ((SceneType)SceneManager.GetActiveScene().buildIndex != SceneType.Level)
            return;

        int tempTier = 0;

        if (state.EquippedSideArmId == _uid && state.UnlockableGunIds.TryGetValue(_uid, out tempTier))
        {
            if (_tier > tempTier)
                return;

            gameObject.SetActive(false);
        }*/
    }

    public override void InitializeGun() { }
    public override void Equip() { }
    public override void Shoot() { }
}
