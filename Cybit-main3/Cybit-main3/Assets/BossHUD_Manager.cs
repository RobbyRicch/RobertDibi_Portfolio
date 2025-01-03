using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHUD_Manager : MonoBehaviour
{
    [Header("Boss Components")]
    [SerializeField] private TextMeshProUGUI _bossTextRef;
    [SerializeField] private Image _bossIcon;

    [Header("HP Bar")]
    [SerializeField] private Slider _bossHPSlider;
    public Slider BossHPSlider { get => _bossHPSlider; set => _bossHPSlider = value; }

    public void InitializeHUD(string bossName, Sprite bossIconSprite)
    {
        _bossTextRef.text = bossName;

        if (bossIconSprite != null)
            _bossIcon.sprite = bossIconSprite;
    }
}
