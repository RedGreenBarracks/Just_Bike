using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

/// <summary>
/// [Just Bike > Setup Start UI] 메뉴로 Canvas + UIManager를 생성합니다.
/// 시작 버튼은 AtomButton(Green)으로 구성됩니다.
/// </summary>
public class SetupStartUI
{
    [MenuItem("Just Bike/Setup Start UI")]
    static void Create()
    {
        var existing = GameObject.Find("UIManager");
        if (existing != null)
            Undo.DestroyObjectImmediate(existing);

        // EventSystem
        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            var esObj = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        // ── UIManager = Canvas ──
        var canvasObj = new GameObject("UIManager");
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create UIManager");

        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();
        var uiManager = canvasObj.AddComponent<UIManager>();

        // ── MenuPanel ──
        var panelObj = CreateUI("MenuPanel", canvasObj.transform);
        var panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        var panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        // ── Title ──
        var titleObj = CreateUI("TitleText", panelObj.transform);
        var titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.65f);
        titleRect.anchorMax = new Vector2(0.5f, 0.65f);
        titleRect.sizeDelta = new Vector2(600, 100);

        var titleText = titleObj.AddComponent<Text>();
        titleText.text = "Just Bike";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 64;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyle.Bold;

        // ── Start Button (AtomButton) ──
        var btnObj = AtomButtonEditor.Create("StartButton", panelObj.transform, ButtonColor.Green, "게임 시작");
        var btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.4f);
        btnRect.anchorMax = new Vector2(0.5f, 0.4f);

        // ── UIManager 참조 연결 ──
        var atomButton = btnObj.GetComponent<AtomButton>();
        var so = new SerializedObject(uiManager);
        so.FindProperty("menuPanel").objectReferenceValue = panelObj;
        so.FindProperty("startButton").objectReferenceValue = atomButton;
        so.ApplyModifiedProperties();

        Selection.activeGameObject = canvasObj;
        Debug.Log("[SetupStartUI] UIManager 생성 완료. 씬을 저장하세요 (Ctrl+S).");
    }

    static GameObject CreateUI(string name, Transform parent)
    {
        var obj = new GameObject(name);
        obj.AddComponent<RectTransform>();
        obj.transform.SetParent(parent, false);
        return obj;
    }
}
