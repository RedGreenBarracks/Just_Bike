using UnityEngine;

/// <summary>
/// ButtonColor Enum에 대응하는 UIColorData를 관리합니다.
/// 씬에 배치하거나 ScriptableObject로 사용하세요.
/// </summary>
[CreateAssetMenu(fileName = "ColorManager", menuName = "Just Bike/Color Manager")]
public class ColorManager : ScriptableObject
{
    [SerializeField] private UIColorData green;
    [SerializeField] private UIColorData blue;
    [SerializeField] private UIColorData red;

    public UIColorData GetColorData(ButtonColor color) => color switch
    {
        ButtonColor.Green => green,
        ButtonColor.Blue => blue,
        ButtonColor.Red => red,
        _ => green
    };
}
