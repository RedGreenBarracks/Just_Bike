using UnityEngine;
using VContainer;

public class UIManager : MonoBehaviour
{
    [Inject] private GameManager gameManager;

    private bool showMenu = true;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;
    private bool stylesInitialized;

    void Start()
    {
        gameManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    void OnGameStateChanged(GameManager.GameState state)
    {
        showMenu = state == GameManager.GameState.Menu;
        Cursor.lockState = showMenu ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showMenu;
    }

    void InitStyles()
    {
        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 64,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.white;

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold
        };

        stylesInitialized = true;
    }

    void OnGUI()
    {
        if (!showMenu) return;
        if (!stylesInitialized) InitStyles();

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height),
            Texture2D.whiteTexture, ScaleMode.StretchToFill, false,
            0, new Color(0.1f, 0.1f, 0.15f, 0.95f), 0, 0);

        float titleW = 600, titleH = 100;
        Rect titleRect = new Rect(
            (Screen.width - titleW) / 2f,
            Screen.height * 0.3f,
            titleW, titleH);
        GUI.Label(titleRect, "Just Bike", titleStyle);

        float btnW = 250, btnH = 60;
        Rect btnRect = new Rect(
            (Screen.width - btnW) / 2f,
            Screen.height * 0.55f,
            btnW, btnH);

        if (GUI.Button(btnRect, "게임 시작", buttonStyle))
        {
            gameManager.StartGame();
        }
    }
}
