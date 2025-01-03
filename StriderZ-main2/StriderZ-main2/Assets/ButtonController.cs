using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1f);
    public float highlightDuration = 0.1f;
    public float highlightedAlpha = 1f;
    public float unselectedAlpha = 0.5f;

    public AudioSource selectSound;

    public RawImage optionalRawImageComponent;
    public TMP_Text optionalTextMeshProComponent;

    private Vector3 defaultScale;
    private bool isHighlighted = false;
    private Color defaultColor;
    private Color defaultTextColor; // Added for TextMeshPro support

    private static ButtonController lastSelectedButton;
    private Graphic buttonGraphic;

    private void Start()
    {
        defaultScale = transform.localScale;
        defaultColor = GetComponent<Button>().colors.normalColor;
        buttonGraphic = GetComponent<Button>().targetGraphic;

        // Store default text color if TextMeshPro component is assigned
        if (optionalTextMeshProComponent != null)
        {
            defaultTextColor = optionalTextMeshProComponent.color;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (isHighlighted)
            return;

        if (lastSelectedButton != null)
            lastSelectedButton.DeselectButton();

        HighlightButton(true);
        ChangeOpacity(highlightedAlpha);
        ScaleButton(targetScale);

        lastSelectedButton = this;

        selectSound.PlayOneShot(selectSound.clip);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DeselectButton();
    }

    private void HighlightButton(bool highlight)
    {
        if (isHighlighted == highlight)
            return;

        isHighlighted = highlight;

        Color targetColor = highlight ? defaultColor : new Color(defaultColor.r, defaultColor.g, defaultColor.b, highlightedAlpha);
        ColorBlock cb = new ColorBlock();
        cb = GetComponent<Button>().colors;
        cb.normalColor = targetColor;
        GetComponent<Button>().colors = cb;

        // Change color of TextMeshPro text if it is assigned
        if (optionalTextMeshProComponent != null)
        {
            Color textMeshProColor = optionalTextMeshProComponent.color;
            textMeshProColor.a = highlight ? highlightedAlpha : unselectedAlpha;
            optionalTextMeshProComponent.color = textMeshProColor;
        }
    }

    private void ScaleButton(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void ChangeOpacity(float alpha)
    {
        Color color = buttonGraphic.color;
        color.a = alpha;
        buttonGraphic.color = color;

        if (optionalRawImageComponent != null)
        {
            Color rawImageColor = optionalRawImageComponent.color;
            rawImageColor.a = alpha;
            optionalRawImageComponent.color = rawImageColor;
        }

        // Change alpha of TextMeshPro text if it is assigned
        if (optionalTextMeshProComponent != null)
        {
            Color textMeshProColor = optionalTextMeshProComponent.color;
            textMeshProColor.a = alpha;
            optionalTextMeshProComponent.color = textMeshProColor;
        }
    }

    private void DeselectButton()
    {
        HighlightButton(false);
        ChangeOpacity(unselectedAlpha);
        ScaleButton(defaultScale);

        // Reset color of TextMeshPro text if it is assigned
        if (optionalTextMeshProComponent != null)
        {
            optionalTextMeshProComponent.color = defaultTextColor;
        }
    }
}