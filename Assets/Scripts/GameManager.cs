using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Menu,
        Game,
        Rizzing,
        GameOver,
        Paused
    }

    [Header("Input")]
    [SerializeField] private InputActionReference escape;

    [SerializeField, HideInInspector] private int menuSceneIndex;
    [SerializeField, HideInInspector] private int gameSceneIndex;

    public int MenuSceneIndex => menuSceneIndex;
    public int GameSceneIndex => gameSceneIndex;

    private GameState _currentState;
    private GameState _savedStateBeforePause;
    private bool _isGamePaused;
    
    protected override void OnAwake()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        if (currentSceneIndex == MenuSceneIndex) 
            ChangeState(GameState.Menu);
        if (currentSceneIndex == GameSceneIndex) 
            ChangeState(GameState.Game);
        
        base.OnAwake();
    }

    void Update()
    {
        if (escape.action.triggered)
        {
            Pause();
        }
    }
    
    public void ChangeState(GameState newState)
    {
        if (newState == _currentState)
        {
            Debug.LogWarning($"Cannot change game state to {newState} because the game is already in that state.");
            return;
        }

        if (newState == GameState.Paused)
        {
            Debug.LogWarning("Cannot change game state to Paused via this function. Use GameManager.Instance.Pause() instead.");
            return;
        }

        if (newState == GameState.Game)
        {
            LockCursor(true);
            
            if (_currentState == GameState.GameOver || _currentState == GameState.Menu)
                SceneManager.LoadScene(gameSceneIndex);
        }
        else if (newState == GameState.Menu)
        {
            LockCursor(false);
            SceneManager.LoadScene(MenuSceneIndex);
        }
        else 
            LockCursor(false);
        
        Debug.Log($"Game state changed from {_currentState} to {newState}.");
        _currentState = newState;
    }

    public void Pause()
    {
        if (_currentState == GameState.Menu)
        {
            Debug.LogWarning("Cannot pause game while in menu.");
            return;
        }

        _isGamePaused = !_isGamePaused;

        if (_isGamePaused)
        {
            _savedStateBeforePause = _currentState;
            _currentState = GameState.Paused;
            Time.timeScale = 0;
        }
        else
        {
            _currentState = _savedStateBeforePause;
            Time.timeScale = 1;
        }
    }
    
    private void LockCursor(bool isCursorLocked)
    {
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLocked;
    }
}