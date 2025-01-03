using UnityEngine;

public enum ColorType { Green, Black, Blue, Pink, Yellow, Red}

public class ColorData : MonoBehaviour
{
    [SerializeField] private int _colorID = 0;
    public int ColorID => _colorID;

    [SerializeField] private Sprite _palletSprite;
    public Sprite PalletSprite => _palletSprite;

    [SerializeField] private Color _iconColor;
    public Color IconColor => _iconColor;

    [Header("Base Color")]
    [SerializeField] private Color _baseBaseColor;
    public Color BaseBaseColor { get => _baseBaseColor; set => _baseBaseColor = value; }

    [ColorUsage(true, true)][SerializeField] private Color _baseEmissionColor;
    public Color BaseEmissionColor { get => _baseEmissionColor; set => _baseEmissionColor = value; }

    [Header("Detail Color")]
    [SerializeField] private Color _detailBaseColor;
    public Color DetailBaseColor { get => _detailBaseColor; set => _detailBaseColor = value; }

    [ColorUsage(true, true)][SerializeField] private Color _detailEmissionColor;
    public Color DetailEmissionColor { get => _detailEmissionColor; set => _detailEmissionColor = value; }

    [Header("Emission Color")]
    [SerializeField] private Color _emissionBaseColor;
    public Color EmissionBaseColor { get => _emissionBaseColor; set => _emissionBaseColor = value; }

    [ColorUsage(true, true)][SerializeField] private Color _emissionEmissionColor;
    public Color EmissionEmissionColor { get => _emissionEmissionColor; set => _emissionEmissionColor = value; }

    [Header("Face Color")]
    [ColorUsage(true, true)][SerializeField] private Color _faceColor;
    public Color FaceColor { get => _faceColor; set => _faceColor = value; }
}
