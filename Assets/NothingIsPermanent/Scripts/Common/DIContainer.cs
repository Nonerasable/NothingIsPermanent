using UnityEngine;
using UnityEngine.SceneManagement;

public class DIContainer : MonoBehaviour {
    
    [SerializeField] private PlayerCanvas _playerUICanvasPrefab;
    [SerializeField] private LevelController _levelController;
    [SerializeField] private bool _isPlayerScene;
    [SerializeField] private string _mainMenuSceneName;
    
    public static DIContainer Inst => _inst;
    public InputSystem_Actions Actions => _actions;
    public LevelController LevelController => _levelController;
    
    private static DIContainer _inst;
    private InputSystem_Actions _actions;
    private PlayerCanvas _currentUiCanvas;
    
    private void Awake() {
        _actions = new();
        _actions.Enable();
        _inst = this;

        if (_isPlayerScene) {
            _currentUiCanvas = Instantiate(_playerUICanvasPrefab);
        }

        _levelController.OnTimeUpdated += HandleTimeUpdate;
        _levelController.OnGameOver += HandleGameOver;
        _levelController.OnLevelWin += HandleGameWin;
        _levelController.OnLevelProgressUpdated += HandleLevelProgressUpdated;
    }

    public void ShowLevelInfo(int levelIndex) {
        _actions.Player.Disable();
        _levelController.SetupLevel(levelIndex);
        var levelSettings = _levelController.GetCurrentLevelSettings();
        _currentUiCanvas.ShowLevelInfo(levelSettings);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartLevel() {
        _currentUiCanvas.StartLevel();
        _levelController.StartCurrentLevel();
        _actions.Player.Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    public void RestartCurrentLevel() {
        var sceneName = _levelController.GetCurrentLevelSettings().LevelSceneName;
        SceneManager.LoadScene(sceneName);
    }

    public void StartNextLevel() {
        var nextLevelSettings = _levelController.GetNextLevelSettings();
        if (nextLevelSettings == null) {
            LoadMainMenu();
            return;
        }
        var sceneName = nextLevelSettings.LevelSceneName;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadTestPreset() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleTimeUpdate(float time) {
        _currentUiCanvas.UpdateTime(time);
    }

    private void HandleGameOver() {
        _actions.Player.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _currentUiCanvas.ShowGameOverPanel();
    }

    private void HandleGameWin() {
        _actions.Player.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _currentUiCanvas.ShowWinLevelPanel();
    }

    private void HandleLevelProgressUpdated(int levelProgress) {
        var levelSettings = _levelController.GetCurrentLevelSettings();
        if (levelSettings == null) {
            return;
        }
        _currentUiCanvas.UpdateProgress(levelProgress, levelSettings.LevelGoal);
    }
}
