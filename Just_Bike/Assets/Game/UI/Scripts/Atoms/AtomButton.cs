using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Atomic Design - Atom 레벨 버튼.
/// Inspector에서 색상(Green/Blue/Red)을 선택하면 자동 적용됩니다.
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class AtomButton : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private ButtonColor buttonColor = ButtonColor.Green;
    [SerializeField] private string labelText = "Button";
    [SerializeField] private int fontSize = 32;

    private Button button;
    private Image background;
    private Text label;

    public Button Button => button;

    void Awake()
    {
        button = GetComponent<Button>();
        background = GetComponent<Image>();
        label = GetComponentInChildren<Text>();
    }

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
        var bg = GetComponent<Image>();
        var btn = GetComponent<Button>();
        if (bg == null || btn == null) return;

        bg.color = buttonColor.ToColor();

        var colors = btn.colors;
        colors.normalColor = buttonColor.ToColor();
        colors.highlightedColor = buttonColor.ToHoverColor();
        colors.pressedColor = buttonColor.ToPressedColor();
        colors.selectedColor = buttonColor.ToColor();
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
