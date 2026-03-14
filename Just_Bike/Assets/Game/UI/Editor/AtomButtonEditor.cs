using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// [Just Bike > Create Atom Button] 메뉴로 AtomButton을 생성합니다.
/// 선택된 Canvas/Panel 하위에 생성됩니다.
/// </summary>
public class AtomButtonEditor
{
    [MenuItem("Just Bike/Create Atom Button")]
    static void CreateAtomButton()
    {
        var parent = Selection.activeTransform;
        if (parent == null || parent.GetComponentInParent<Canvas>() == null)
        {
            Debug.LogWarning("[AtomButton] Canvas 하위 오브젝트를 선택한 뒤 실행하세요.");
            return;
        }

        var btnObj = Create("AtomButton", parent, ButtonColor.Green, "Button");
        Selection.activeGameObject = btnObj;
    }

    /// <summary>
    /// AtomButton을 코드에서 생성할 때 사용합니다.
    /// </summary>
    public static GameObject Create(string name, Transform parent, ButtonColor color, string label)
    {
        var btnObj = new GameObject(name);
        btnObj.AddComponent<RectTransform>();
        btnObj.transform.SetParent(parent, false);

        var rect = btnObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250, 60);

        btnObj.AddComponent<Image>();
        btnObj.AddComponent<Button>();

        var atom = btnObj.AddComponent<AtomButton>();

        // Label Text (자식)
        var textObj = new GameObject("Label");
        textObj.AddComponent<RectTransform>();
        textObj.transform.SetParent(btnObj.transform, false);

        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.fontStyle = FontStyle.Bold;

        // SerializedObject로 설정 적용
        var so = new SerializedObject(atom);
        so.FindProperty("buttonColor").enumValueIndex = (int)color;
        so.FindProperty("labelText").stringValue = label;
        so.ApplyModifiedProperties();

        // OnValidate 트리거
        atom.SetColor(color);
        atom.SetLabel(label);

        Undo.RegisterCreatedObjectUndo(btnObj, "Create AtomButton");
        return btnObj;
    }
}
