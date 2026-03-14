using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// Canvas 오브젝트에 직접 붙여서 사용합니다.
/// Editor 메뉴 [Just Bike > Setup Start UI]로 자동 생성 가능.
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class UIManager : MonoBehaviour
{
    [Inject] private GameManager gameManager;

    [Header("UI 참조")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private AtomButton startButton;

    void Start()
    {
        gameManager.OnGameStateChanged += OnGameStateChanged;

        if (startButton != null)
            startButton.Button.onClick.AddListener(OnStartClicked);
    }

    void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnGameStateChanged -= OnGameStateChanged;

        if (startButton != null)
            startButton.Button.onClick.RemoveListener(OnStartClicked);
    }

    void OnStartClicked()
    {
        gameManager.StartGame();
    }

    void OnGameStateChanged(GameManager.GameState state)
    {
        bool isMenu = state == GameManager.GameState.Menu;

        if (menuPanel != null)
            menuPanel.SetActive(isMenu);

        Cursor.lockState = isMenu ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isMenu;
    }
}
