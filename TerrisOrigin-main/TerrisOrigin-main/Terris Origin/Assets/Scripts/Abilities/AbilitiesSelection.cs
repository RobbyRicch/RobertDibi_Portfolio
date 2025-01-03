using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesSelection : MonoBehaviour
{
    public enum ElementType
    {
        Lightning,
        Fire,
        Light,
        Earth,
        Darkness,
        Water
    }
    public enum StackedElement
    {
        noStacking = 0,
        LightANDFire = 4,
    }

    public ElementType RightHandElement;
    public ElementType LeftHandElement;
    public StackedElement StackingEffect;

    public static AbilitiesSelection Instance;
    [SerializeField] private InputManager inputManager;

    private GameObject abiliesObject;
    private BasicAttack basicAttack;
    private SecondaryBasicAttack secondaryBasic;
    [SerializeField] private List<SerializedDictionary> RightSleeves = new List<SerializedDictionary>();
    [SerializeField] private List<SerializedDictionary> LeftSleeves = new List<SerializedDictionary>();

    //Lightning
    private LightningDash lighningDash;
    private LightningAOE lightningWrath;

    //Fire
    private FlameAura FireAura;
    private CanonBall canonBall;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        abiliesObject = GameObject.FindGameObjectWithTag("Abilities");
        basicAttack = abiliesObject.GetComponent<BasicAttack>();
        secondaryBasic = abiliesObject.GetComponent<SecondaryBasicAttack>();
        lighningDash = abiliesObject.GetComponent<LightningDash>();
        lightningWrath = abiliesObject.GetComponent<LightningAOE>();
        FireAura = abiliesObject.GetComponent<FlameAura>();
        canonBall = abiliesObject.GetComponent<CanonBall>();

        LeftHandChange();
        RightHandChange();
    }

    [ContextMenu("LeftHandChange")]
    public void LeftHandChange()
    {
        inputManager.ResetValues();
        switch (LeftHandElement)
        {
            case ElementType.Lightning:
                lighningDash.enabled = true;
                lightningWrath.enabled = true;
                FireAura.enabled = false;
                canonBall.enabled = false;
                for (int i = 0; i < LeftSleeves.Count; i++)
                {
                    if (LeftSleeves[i].name == "Lightning")
                        LeftSleeves[i].gameObject.SetActive(true);
                    else
                        LeftSleeves[i].gameObject.SetActive(false);
                }
                break;
            case ElementType.Fire:
                lighningDash.enabled = false;
                lightningWrath.enabled = false;
                FireAura.enabled = true;
                canonBall.enabled = true;
                for (int i = 0; i < LeftSleeves.Count; i++)
                {
                    if (LeftSleeves[i].name == "Fire")
                        LeftSleeves[i].gameObject.SetActive(true);
                    else
                        LeftSleeves[i].gameObject.SetActive(false);
                }
                break;
            case ElementType.Light:
                break;
            case ElementType.Earth:
                break;
            case ElementType.Darkness:
                break;
            case ElementType.Water:
                break;
        }
        CalcSTelement();
        inputManager.enabled = false;
        inputManager.enabled = true;
    }
    [ContextMenu("RightHandChange")]
    public void RightHandChange()
    {
        inputManager.ResetValues();
        basicAttack.ChangeElement();
        secondaryBasic.ChangeElement();
        switch (RightHandElement)
        {
            case ElementType.Lightning:
                for (int i = 0; i < RightSleeves.Count; i++)
                {
                    if (RightSleeves[i].name == "Lightning")
                        RightSleeves[i].gameObject.SetActive(true);
                    else
                        RightSleeves[i].gameObject.SetActive(false);
                }
                break;
            case ElementType.Fire:
                for (int i = 0; i < RightSleeves.Count; i++)
                {
                    if (RightSleeves[i].name == "Fire")
                        RightSleeves[i].gameObject.SetActive(true);
                    else
                        RightSleeves[i].gameObject.SetActive(false);
                }
                break;
            case ElementType.Light:
                break;
            case ElementType.Earth:
                break;
            case ElementType.Darkness:
                break;
            case ElementType.Water:
                break;
        }
        CalcSTelement();
    }
    private void CalcSTelement()
    {
        int x = (int)RightHandElement, y = (int)LeftHandElement;
        if (x > y)
        {
            int swap = y;
            y = x;
            x = swap;
        }
        if (RightHandElement == LeftHandElement)
            StackingEffect = 0;
        else
            StackingEffect = (StackedElement)(Mathf.Pow(2, x) + Mathf.Pow(3, y));
    }
}
