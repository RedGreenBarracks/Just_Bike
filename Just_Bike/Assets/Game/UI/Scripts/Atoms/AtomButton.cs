using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Atomic Design - Atom 레벨 버튼.
/// ColorManager를 Inspector에서 연결하면 Editor/런타임 모두 색상 적용.
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class AtomButton : MonoBehaviour
{
    [Header("색상")]
    [SerializeField] private ColorManager colorManager;
    [SerializeField] private ButtonColor buttonColor = ButtonColor.Green;

    [Header("텍스트")]
    [SerializeField] private string labelText = "Button";
    [SerializeField] private int fontSize = 32;

    public Button Button => GetComponent<Button>();

    void OnValidate()
    {
        ApplyColor();
        ApplyLabel();
    }

    public void SetColor(ButtonColor color)
    {
        buttonColor = color;
        ApplyColor();
    }

    public void SetLabel(string text)
    {
        labelText = text;
        ApplyLabel();
    }

    void ApplyColor()
    {
        if (colorManager == null) return;

        var bg = GetComponent<Image>();
        var btn = GetComponent<Button>();
        if (bg == null || btn == null) return;

        var data = colorManager.GetColorData(buttonColor);
        if (data == null) return;

        bg.color = data.Normal;

        var colors = btn.colors;
        colors.normalColor = data.Normal;
        colors.highlightedColor = data.Hover;
        colors.pressedColor = data.Pressed;
        colors.selectedColor = data.Normal;
        colors.fadeDuration = 0.1f;
        btn.colors = colors;
    }

    void ApplyLabel()
    {
        var txt = GetComponentInChildren<Text>();
        if (txt == null) return;

        txt.text = labelText;
        txt.fontSize = fontSize;
    }
}
