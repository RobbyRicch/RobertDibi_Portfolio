using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CutsceneManager : MonoBehaviour // , IInjectable
{
    [Header("References")]
    //public ScriptTriggerManager scriptManagerRef;
    public CinemachineVirtualCamera cameraRef;
    public Animator camAnimator;
    private Transform originalTarget;

    [Header("Targets")]
    public Transform targetPosition;
    public float targetSize = 5;

    [Header("Activate/Disable")]
    public bool activateCutscene;
    public bool cutsceneFinished;

   public void ChangeCamTarget()
    {
        cameraRef.Follow = targetPosition;
        camAnimator.SetBool("Zoom",true);
    }
    public void ResetCam()
    {
        cameraRef.Follow = originalTarget;
        camAnimator.SetBool("Zoom", false);
    }

    public void OnStartRun()
    {

    }

    /*public void Inject() // need to see why I did this
    {
        bool isVirtualCamera = false;
        List<GameObject> persistables = SaveManager.Instance.Persistables;
        for (int i = 0; i < persistables.Count; i++)
        {
            isVirtualCamera = persistables[i].TryGetComponent(out CinemachineVirtualCamera virtualCamera);
            if (isVirtualCamera)
            {
                cameraRef = virtualCamera;
                camAnimator = virtualCamera.GetComponent<Animator>();
            }
        }

        originalTarget = cameraRef.Follow;
    }*/
}




