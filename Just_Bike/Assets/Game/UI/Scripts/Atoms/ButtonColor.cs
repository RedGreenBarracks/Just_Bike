using UnityEngine;

public enum ButtonColor
{
    Green,
    Blue,
    Red
}

public static class ButtonColorExtensions
{
    public static Color ToColor(this ButtonColor color) => color switch
    {
        ButtonColor.Green => new Color(0.2f, 0.6f, 0.3f),
        ButtonColor.Blue => new Color(0.2f, 0.4f, 0.8f),
        ButtonColor.Red => new Color(0.8f, 0.25f, 0.25f),
        _ => Color.gray
    };

    public static Color ToHoverColor(this ButtonColor color) => color switch
    {
        ButtonColor.Green => new Color(0.25f, 0.7f, 0.35f),
        ButtonColor.Blue => new Color(0.25f, 0.5f, 0.9f),
        ButtonColor.Red => new Color(0.9f, 0.3f, 0.3f),
        _ => Color.gray
    };

    public static Color ToPressedColor(this ButtonColor color) => color switch
    {
        ButtonColor.Green => new Color(0.15f, 0.5f, 0.25f),
        ButtonColor.Blue => new Color(0.15f, 0.3f, 0.7f),
        ButtonColor.Red => new Color(0.65f, 0.2f, 0.2f),
        _ => Color.gray
    };
}
