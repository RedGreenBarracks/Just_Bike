using UnityEngine;

/// <summary>
/// 1개 색상의 상태별(Normal/Hover/Pressed) 컬러를 정의합니다.
/// [Create > Just Bike > UI Color Data]로 에셋 생성 가능.
/// </summary>
[CreateAssetMenu(fileName = "NewColor", menuName = "Just Bike/UI Color Data")]
public class UIColorData : ScriptableObject
{
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color pressedColor = Color.gray;

    public Color Normal => normalColor;
    public Color Hover => hoverColor;
    public Color Pressed => pressedColor;
}
