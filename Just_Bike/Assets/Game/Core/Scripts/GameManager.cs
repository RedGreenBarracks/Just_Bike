using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Menu, Playing }
    public GameState CurrentState { get; private set; } = GameState.Menu;

    public event System.Action<GameState> OnGameStateChanged;

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void ReturnToMenu()
    {
        CurrentState = GameState.Menu;
        OnGameStateChanged?.Invoke(CurrentState);
    }
}
