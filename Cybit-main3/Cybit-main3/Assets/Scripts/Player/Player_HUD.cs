using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_HUD : MonoBehaviour
{
    [Header("UI Video Overlays")]
    [SerializeField] private GameObject _glitchOverlay;
    public GameObject GlitchOverlay => _glitchOverlay;

    [SerializeField] private GameObject _damageOverlay;
    public GameObject DamageOverlay => _damageOverlay;

    [SerializeField] private GameObject _backgroundFade;
    public GameObject BackgroundFade => _backgroundFade;

    [Header("Link Integrity")]
    [SerializeField] private GameObject _linkIntegrityGroup;
    public GameObject LinkIntegrityGroup => _linkIntegrityGroup;

    [Header("Health Slots")]
    [SerializeField] private List<GameObject> _hpSlots;
    [SerializeField] private GameObject _hpHudGroup;

    public GameObject HealthHudGroup => _hpHudGroup;

    [Header("Currency")]
    [SerializeField] private GameObject _currencyGroup;
    public GameObject CurrencyGroup => _currencyGroup;

    [SerializeField] private Image _currencyIcon;
    public Image CurrencyIcon => _currencyIcon;

    [SerializeField] private TextMeshProUGUI _currencyTxt;
    public TextMeshProUGUI CurrencyTxt => _currencyTxt;

    [SerializeField] private TextMeshProUGUI _currencySubTxt;
    public TextMeshProUGUI CurrencySubTxt => _currencySubTxt;

    [SerializeField] private TextMeshProUGUI _currencySubTxtPlus;
    public TextMeshProUGUI CurrencySubTxtPlus => _currencySubTxtPlus;

    [SerializeField] private TextMeshProUGUI _currencySubTxtMinus;
    public TextMeshProUGUI CurrencySubTxtMinus => _currencySubTxtMinus;

    [Header("Health Bar")]
    //[SerializeField] private Image[] _healthBarBg;
    [SerializeField] private GameObject _attributesGroup;
    public GameObject AttributesGroup => _attributesGroup;
    //[SerializeField] private Image _healthBarImg; 
    [SerializeField] private float _damagePopTime = 0.5f, _damageShrinkTime = 0.5f;


    [Header("Stamina Bar")]
    [SerializeField] private Image[] _staminaBarBg;
    [SerializeField] private Image _staminaBarImg;
    [SerializeField] private GameObject _staminaHudGroup;
    public GameObject StaminaHudGroup => _staminaHudGroup;


    [Header("Focus")]
    [SerializeField] private Image _focusBg;
    [SerializeField] private Image _focusFillImg;
    [SerializeField] private GameObject _focusHudGroup;

    public GameObject FocusHudGroup => _focusHudGroup;

    [Header("Deflect")]
    [SerializeField] private GameObject _deflectHudGroup;

    public GameObject DeflectHudGroup => _deflectHudGroup;

    [Header("Ultimate")]
    [SerializeField] private GameObject _ultBarGroup;
    public GameObject UltBarGroup => _ultBarGroup;

    [SerializeField] private Image _ultBg;
    [SerializeField] private Image _ultFillImg;

    [Header("Primary Weapon")]
    [SerializeField] private GameObject _primaryWeaponHUD;
    [SerializeField] private Image _mainWepSprite;
    [SerializeField] private TextMeshProUGUI _mainWepMaxAmmoTXT;
    [SerializeField] private TextMeshProUGUI _mainWepCurrentAmmoTXT;

    [Header("Secondary Weapon")]
    [SerializeField] private GameObject _secondaryWeaponHUD;
    [SerializeField] private Image _secWepSprite;
    [SerializeField] private TextMeshProUGUI _secWepMaxAmmoTXT;
    [SerializeField] private TextMeshProUGUI _secWepCurrentAmmoTXT;

    [Header("Other / Fluff")]
    [SerializeField] private GameObject _hazyVisionGO;
    
    public GameObject HazyVisionGO => _hazyVisionGO;
    public GameObject DeathMenu;

    private bool IsNotFadingOrVisible(TextMeshProUGUI text)
    {
        // Placeholder logic for checking if faded in
        return text.color.a != 0;
    }
    public IEnumerator UIPopEffect(Transform bg, float popScale, float popDuration, float shrinkDurarion)
    {
        float elapsedTime = 0f;
        Vector3 originalScale = Vector3.one;

        while (elapsedTime < popDuration)
        {
            bg.transform.localScale = Vector3.Lerp(originalScale, originalScale * popScale, elapsedTime / popDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bg.transform.localScale = originalScale * popScale;

        elapsedTime = 0f;
        while (elapsedTime < shrinkDurarion)
        {
            bg.transform.localScale = Vector3.Lerp(originalScale * popScale, originalScale, elapsedTime / shrinkDurarion);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bg.transform.localScale = originalScale;
    }

    /*private IEnumerator UIDamageEffect(float timeToWait, float popScale)
    {
        StartCoroutine(UIPopEffect(_healthBarBg[0].transform.parent, popScale, timeToWait / 4, timeToWait / 4 * 3));
        for (int i = 0; i < _healthBarBg.Length; i++)
            _healthBarBg[i].color = Color.red;

        yield return new WaitForSecondsRealtime(timeToWait);

        for (int i = 0; i < _healthBarBg.Length; i++)
            _healthBarBg[i].color = Color.white;
    }*/
    /*public void UpdateDamage(float timeToWait, float popScale)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(UIDamageEffect(timeToWait, popScale));
    }*/

    public void UpdateCurrency(int newCurrency)
    {
        _currencyTxt.text = newCurrency.ToString();
    }

    public void UpdateHealthSlots(float currentHealth)
    {
        for (int i = 0; i < _hpSlots.Count; i++)
        {
            _hpSlots[i].SetActive(i < currentHealth);
        }
    }
    public void UpdateStaminaBar(int maxStamina, float currentStamina)
    {
        float fillAmount = currentStamina / maxStamina;
        _staminaBarImg.fillAmount = fillAmount;
    }
    public void UpdateUltBar(int maxUltCharge, float currentUltCharge)
    {
        float fillAmount = currentUltCharge / maxUltCharge;
        _ultFillImg.fillAmount = fillAmount;
    }
    public void UpdateFocusBar(int maxFocus, float currentFocus)
    {
        float fillAmount = currentFocus / maxFocus;
        _focusFillImg.fillAmount = fillAmount;
    }
    public void UpdatePrimaryGun(GunPrimary primary)
    {
        if (!primary)
        {
            _primaryWeaponHUD.SetActive(false);
            return;
        }

        _primaryWeaponHUD.SetActive(true);
        _mainWepSprite.sprite = primary.SR.sprite;
        _mainWepMaxAmmoTXT.text = primary.MaxClipSize.ToString();

        if (primary is GunMinigun) // quick and dirty fix
            _mainWepCurrentAmmoTXT.text = (primary.MaxClipSize - primary.CurrentClipSize).ToString();
        else
            _mainWepCurrentAmmoTXT.text = primary.CurrentClipSize.ToString();
    }
    public void UpdateSideArm(GunSideArm sideArm)
    {
        if (!sideArm)
        {
            _secondaryWeaponHUD.SetActive(false);
            return;
        }

        _secondaryWeaponHUD.SetActive(true);
        _secWepSprite.sprite = sideArm.SR.sprite;
        _secWepMaxAmmoTXT.text = sideArm.MaxClipSize.ToString();
        _secWepCurrentAmmoTXT.text = sideArm.CurrentClipSize.ToString();
    }
}
