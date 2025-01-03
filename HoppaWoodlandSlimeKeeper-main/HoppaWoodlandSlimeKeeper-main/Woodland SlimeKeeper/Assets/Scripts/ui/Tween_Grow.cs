using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween_Grow : MonoBehaviour
{

    [SerializeField]
    private float duration=2f;
    [SerializeField]
    private LeanTweenType easeType;
   // [SerializeField]
    //private int delay=0;
    [SerializeField]
    private float scaleSize=2;
    [SerializeField]
   // private bool doSetLoop;
   // [SerializeField]
   // private LeanTweenType loopType;
    void Start()
    {
        //if (doSetLoop)
        //    LeanTween.scale(gameObject, new Vector3(scaleSize, scaleSize, scaleSize), duration).setEase(easeType);//.setDelay(3)..setLoopType(loopType);
        //else
        LeanTween.scale(gameObject, transform.localScale * scaleSize, duration).setEase(easeType);//.setOnComplete(ScaleDown);
        //LeanTween.rotateY(gameObject, 180, duration * 0.5f);
    }

    public void ScaleDown()
    {
        LeanTween.scale(gameObject, transform.localScale / scaleSize, duration).setEase(easeType);
    }
  
    public void SetTweenFXToGO_scale(GameObject _go)
    {
        
        LeanTween.scale(_go, transform.localScale * scaleSize, duration).setEase(LeanTweenType.punch);
    }
}
