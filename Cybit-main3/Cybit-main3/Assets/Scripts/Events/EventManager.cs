using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

//public interface IInteractable { }

public static class EventManager
{
    #region Save & Load Events
    public static Action<List<Profile>> OnProfilesLoaded;
    #endregion

    /*#region Menu Related Events
    public static Action OnMainMenuInitialized;
    #endregion*/

    #region Run Related Events
    public static Action OnStartRun;
    public static Action OnEndRun;
    #endregion

    public static Action OnPauseGame;
    public static Action OnInteract;
    public static Action OnCameraShake;
    public static Action OnTutorialComplete;
    public static Action OnUpdateCurrency;
    public static Action OnEndGame;
    public static Action<int> OnGainCurrency;
    public static Action<int> OnPayCurrency;
    public static Action<float> OnTimeFreeze, OnGainFocus, OnPickupConsumable;
    public static Action<bool> OnFocus;
    public static Action<bool> OnCutscene;
    public static Action<Vector2, float, float> OnPlayerHit;
    public static Action OnHitDummy;
    public static Action<StatType, float> OnSkillAquired;
    public static Action<string> OnNewObjective;
    public static Action<bool> OnCloseObjective;
    public static Action<bool> OnReOpenObjective;
    public static Action<bool, string, AudioClip> OnUpdateStatusEffect;
    public static Action<LinkIntegritySystem> OnSoftReset;
    public static Action<Player_Controller> OnVendingMachineInteract;
    public static Action OnInitializeLink;
    public static Action<Player_Controller> OnMouseSensetivityChange;
    public static Action<Barrier> OnBarrierDown;
    public static Action<EnemyBase> OnEnemyDeath;
    public static Action<GunBase> OnGunUpgrade;

    #region Save & Load Invoke Methods
    public static void InvokeProfilesLoaded(List<Profile> profilesLoaded)
    {
        if (profilesLoaded == null || profilesLoaded.Count < 1) return;

        OnProfilesLoaded?.Invoke(profilesLoaded);
        Debug.Log("Event: ProfilesLoaded");
    }
    #endregion

    /*#region Menu Related Methods
    public static void InvokeMainMenuInitialized()
    {
        OnMainMenuInitialized?.Invoke();
        Debug.Log("Event: MainMenuInitialized");
    }
    #endregion*/

    #region Run Related Invoke Methods
    public static void InvokeStartRun()
    {
        OnStartRun?.Invoke();
        Debug.Log("Event: StartRun");
    }
    public static void InvokeEndRun()
    {
        OnEndRun?.Invoke();
        Debug.Log("Event: EndRun");
    }
    #endregion

    public static void InvokePause()
    {
        OnPauseGame?.Invoke();
        Debug.Log("Event: Pause");
    }
    public static void InvokeInteract()
    {
        OnInteract?.Invoke();
        Debug.Log("Event: Interact");
    }
    public static void InvokeCameraShake()
    {
        OnCameraShake?.Invoke();
        Debug.Log("Event: CameraShake");
    }
    public static void InvokeTutorialComplete()
    {
        OnTutorialComplete?.Invoke();
        Debug.Log("Event: TutorialComplete");
    }
    public static void InvokeHitDummy()
    {
        OnHitDummy?.Invoke();
        Debug.Log("Event: HitDummy");
    }

    public static void InvokeEndGame()
    {
        OnEndGame?.Invoke();
        Debug.Log("Event: EndGame");
    }
    public static void InvokeTimeFreeze(float timeToFreeze)
    {
        OnTimeFreeze?.Invoke(timeToFreeze);
        Debug.Log("Event: TimeFreeze for " + timeToFreeze + " in seconds");
    }
    public static void InvokeGainFocus(float focusToIncrease)
    {
        if (focusToIncrease < 1)
            return;

        OnGainFocus?.Invoke(focusToIncrease);
        Debug.Log("Event: GainFocus");
    }
    public static void InvokeUpdateCurrency()
    {
        OnUpdateCurrency?.Invoke();
        Debug.Log("Event: UpdateCurrency");
    }
    public static void InvokeSkillAquired(StatType statType, float value)
    {
        OnSkillAquired?.Invoke(statType, value);
        Debug.Log("Event: SkillAquired, " + statType.ToString());
    }
    public static void InvokeGainCurrency(int amount)
    {
        OnGainCurrency?.Invoke(amount);
        Debug.Log("Event: GainCurrency");
    }
    public static void InvokePayCurrency(int amount)
    {
        if (amount < 1)
            return;

        OnPayCurrency?.Invoke(amount);
        Debug.Log("Event: PayCurrency");
    }
    public static void InvokePickupConsumable(float value)
    {
        if (value < 1)
            return;

        OnPickupConsumable?.Invoke(value);
        Debug.Log("Event: PickupConsumable");
    }
    public static void InvokeNewObjective(string objectiveText)
    {
        if (objectiveText == string.Empty)
            return;

        OnNewObjective?.Invoke(objectiveText);
        Debug.Log("Event: NewObjectiveText");
    }

    public static void InvokeCloseObjective(bool close)
    {
        if (close)
        {
            OnCloseObjective?.Invoke(close);
            Debug.Log("Event: CloseObjective");

        }

    }

    public static void InvokeReOpenObjective(bool open)
    {
        if (open)
        {
            OnReOpenObjective?.Invoke(open);
            Debug.Log("Event: ReOpenObjective");

        }
    }

    public static void InvokeFocus(bool isStarting)
    {
        OnFocus?.Invoke(isStarting);
        Debug.Log("Event: Focus");
    }
    public static void InvokeCutscene(bool isStarting)
    {
        string detail = isStarting ? "Start" : "End";
        OnCutscene?.Invoke(isStarting);
        Debug.Log("Event: Cutscene " + detail);
    }
    public static void InvokeUpdateStatusEffect(bool isPositiveEffect, string statusText, AudioClip statusRelatedClip)
    {
        OnUpdateStatusEffect?.Invoke(isPositiveEffect, statusText, statusRelatedClip);
        Debug.Log("Event: Update Status Effect ");
    }

    public static void InvokePlayerHit(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        if (damage < 1)
            return;

        OnPlayerHit?.Invoke(normalizedAttackDirection, damage, knockBackPower);
        Debug.Log("Event: HitPlayer");
    }
    public static void InvokeSoftReset(LinkIntegritySystem LIS)
    {
        if (LIS == null)
            return;

        OnSoftReset?.Invoke(LIS);
        Debug.Log("Event: SoftReset");
    }
    public static void InvokeVendingMachineInteract(Player_Controller player)
    {
        if (!player) return;

        OnVendingMachineInteract?.Invoke(player);
        Debug.Log("Event: VendingMachineInteract");
    }
    public static void InvokeInitializeLink()
    {
        OnInitializeLink?.Invoke();
        Debug.Log("Event: InitializeLink");
    }
    public static void InvokeMouseSensetivityChange(Player_Controller controller)
    {
        if (controller == null)
            return;

        OnMouseSensetivityChange?.Invoke(controller);
        Debug.Log("Event: GameLaunched");
    }
    public static void InvokeBarrierDown(Barrier barrier)
    {
        if (barrier == null)
            return;

        OnBarrierDown?.Invoke(barrier);
        Debug.Log("Event: BarrierDown");
    }
    public static void InvokeEnemyDeath(EnemyBase enemy)
    {
        if (enemy == null)
            return;

        SaveManager saveManagerInstance = SaveManager.Instance;
        Player_Controller playerController = SaveManager.Instance.Player;
        playerController.CurrentKills++;
        OnEnemyDeath?.Invoke(enemy);
        Debug.Log("Event: EnemyDeath");

        if (playerController.CurrentKills > playerController.KillsToUpgrade)
        {
            //InvokeGunUpgrade(playerController.CurrentlyEquippedGun); gun upgrade
        }
    }
    public static void InvokeGunUpgrade(GunBase gun)
    {
        if (gun == null)
            return;

        OnGunUpgrade?.Invoke(gun);
        Debug.Log("Event: GunUpgrade");
    }
}
