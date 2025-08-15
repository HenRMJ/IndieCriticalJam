using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColorChanger : MonoBehaviour
{
    [SerializeField] private bool isMaterial = false;

    private Color primaryColor;
    private Color secondaryColor;

    private Image _image;
    private Material _material;

    private void Awake()
    {
        _image = GetComponent<Image>();

        if (isMaterial)
        {
            _material = _image.material;
        }
    }

    public void ChangeColor(Color primary, Color secondary)
    {
        primaryColor = primary;
        secondaryColor = secondary;

        if (isMaterial)
        {
            _material.SetColor("_Color", primaryColor);
            _material.SetColor("_StripColor", secondaryColor);
        }
        else
        {
            _image.color = primaryColor;
        }
    }

    public Color GetPrimaryColor() => primaryColor;
    public Color GetSecondaryColor() => secondaryColor;
}
