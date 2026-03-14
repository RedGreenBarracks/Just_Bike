using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// AtomButton 관련 Editor 기능.
/// - [Just Bike > Create Atom Button] — 선택한 Canvas/Panel 하위에 AtomButton 생성
/// - [Just Bike > Create Atom Button Prefab] — Prefabs/Atoms 폴더에 Prefab 에셋 생성
/// </summary>
public class AtomButtonEditor
{
    const string PrefabFolder = "Assets/Game/UI/Prefabs/Atoms";
    const string PrefabPath = PrefabFolder + "/AtomButton.prefab";

    [MenuItem("Just Bike/Create Atom Button")]
    static void CreateAtomButton()
    {
        var parent = Selection.activeTransform;
        if (parent == null || parent.GetComponentInParent<Canvas>() == null)
        {
            Debug.LogWarning("[AtomButton] Canvas 하위 오브젝트를 선택한 뒤 실행하세요.");
            return;
        }

        // Prefab이 있으면 Prefab Instance로 생성
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab != null)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            Undo.RegisterCreatedObjectUndo(instance, "Create AtomButton");
            Selection.activeGameObject = instance;
            Debug.Log("[AtomButton] Prefab Instance로 생성되었습니다.");
        }
        else
        {
            var btnObj = CreateRaw("AtomButton", parent, ButtonColor.Green, "Button");
            Selection.activeGameObject = btnObj;
            Debug.Log("[AtomButton] 생성 완료. Prefab이 없어서 일반 오브젝트로 생성되었습니다.");
        }
    }

    [MenuItem("Just Bike/Create Atom Button Prefab")]
    static void CreatePrefab()
    {
        // 임시 오브젝트 생성
        var tempObj = CreateRaw("AtomButton", null, ButtonColor.Green, "Button");

        // 폴더 생성
        if (!AssetDatabase.IsValidFolder("Assets/Game/UI/Prefabs"))
            AssetDatabase.CreateFolder("Assets/Game/UI", "Prefabs");
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
            AssetDatabase.CreateFolder("Assets/Game/UI/Prefabs", "Atoms");

        // Prefab 저장
        var prefab = PrefabUtility.SaveAsPrefabAsset(tempObj, PrefabPath);
        Object.DestroyImmediate(tempObj);

        if (prefab != null)
        {
            Selection.activeObject = prefab;
            Debug.Log("[AtomButton] Prefab 생성 완료: " + PrefabPath);
        }
    }

    /// <summary>
    /// AtomButton 오브젝트를 직접 생성합니다 (Prefab 없이).
    /// </summary>
    public static GameObject CreateRaw(string name, Transform parent, ButtonColor color, string label)
    {
        var btnObj = new GameObject(name);
        btnObj.AddComponent<RectTransform>();
        if (parent != null)
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

        var so = new SerializedObject(atom);
        so.FindProperty("buttonColor").enumValueIndex = (int)color;
        so.FindProperty("labelText").stringValue = label;
        so.ApplyModifiedProperties();

        atom.SetColor(color);
        atom.SetLabel(label);

        Undo.RegisterCreatedObjectUndo(btnObj, "Create AtomButton");
        return btnObj;
    }
}
