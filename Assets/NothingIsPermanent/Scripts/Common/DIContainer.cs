using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DIContainer : MonoBehaviour {
    
    [SerializeField] private PlayerCanvas _playerUICanvasPrefab;
    [SerializeField] private LevelController _levelController;
    [SerializeField] private bool _isPlayerScene;
    [SerializeField] private string _mainMenuSceneName;

    public MicrobeColoring microbeColoring;

    [HideInInspector] public GameObject Player;
    public MicrobeGlobalParams MicrobeGlobalParams = new();
    
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

        Actions.UI.Esc.performed += EscMenuPerformed;
        Actions.UI.Tab.performed += UpgradeMenuPerformed;

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

    public void OpenEscMenu() {
        _actions.Player.Disable();
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _currentUiCanvas.ShowEscMenu();
    }

    public void OpenUpgradePanel() {
        _actions.Player.Disable();
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _currentUiCanvas.ShowUpgradePanel();
    }

    public void Resume() {
        _actions.Player.Enable();
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _currentUiCanvas.HidePausePanels();
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

    public void ChangeMicrobe(Microbe microbe) {
        if (microbe == null) {
            _currentUiCanvas.PlayerPanel.HideUsableHint();
        }
        else {
            _currentUiCanvas.PlayerPanel.ShowUsableHint("E" , "Collect the microbes in a flask");
        }
    }

    public void ChangeMicrobeCollection(DestructibleMaterialType destructibleMaterialType, List<Microbe> microbeList) {
        var collectedMicrobeCount = 0;
        foreach (var microbe in microbeList) {
            if (microbe.IsCollected) {
                collectedMicrobeCount += 1;
            }
        }
        _currentUiCanvas.PlayerPanel.UpdateMicrobeAmmo(collectedMicrobeCount, microbeList.Count, destructibleMaterialType);
    }

    private void EscMenuPerformed(InputAction.CallbackContext obj) {
        if (Time.timeScale == 0f) {
            Resume();
        }
        else {
            OpenEscMenu();
        }
    }

    private void UpgradeMenuPerformed(InputAction.CallbackContext obj) {
        if (Time.timeScale == 0f) {
            Resume();
        }
        else {
            OpenUpgradePanel();
        }
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
