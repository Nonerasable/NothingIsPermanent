using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DIContainer : MonoBehaviour {
    
    [SerializeField] private PlayerCanvas _playerUICanvasPrefab;
    [SerializeField] private LevelController _levelController;
    [SerializeField] private List<string> levelSceneNames;
    [SerializeField] private string _mainMenuSceneName;
    [SerializeField] private FloatingTextPool _floatingTextPool;
    [SerializeField] private SceneLoadHandler _sceneLoadHandler;
    [SerializeField] private EventSystem _eventSystemPrefab;

    public MicrobeColoring microbeColoring;

    [HideInInspector]
    public GameObject Player {
        get => _player;
        set {
            _player = value;
            if (_player == null) {
                return;
            }
            _player.GetComponent<MicrobeController>().SetupMicrobe(_currentLevelSettings?.MicrobeSettings);
            ProgressionController.UpdateUi();
        }
    }
    public MicrobeGlobalParams MicrobeGlobalParams = new();
    public MicrobeProgressionController ProgressionController = new();
    
    public static DIContainer Inst => _inst;
    public InputSystem_Actions Actions => _actions;
    public LevelController LevelController => _levelController;
    public FloatingTextPool FloatingTextPool => _floatingTextPool;

    private GameObject _player;
    private static DIContainer _inst;
    private InputSystem_Actions _actions;
    private PlayerCanvas _currentUiCanvas;
    private LevelSettings _currentLevelSettings;
    
    private void Awake() {
        if (_inst != null) {
            Destroy(gameObject);
            return;
        }
        
        _actions = new();
        _actions.Enable();
        _inst = this;

        _currentUiCanvas = Instantiate(_playerUICanvasPrefab);
        Instantiate(_eventSystemPrefab, transform);

        Actions.UI.Esc.performed += EscMenuPerformed;
        Actions.UI.Tab.performed += UpgradeMenuPerformed;

        _levelController.OnTimeUpdated += HandleTimeUpdate;
        _levelController.OnGameOver += HandleGameOver;
        _levelController.OnLevelWin += HandleGameWin;
        _levelController.OnLevelProgressUpdated += HandleLevelProgressUpdated;

        _sceneLoadHandler.OnSceneLoaded += HandleSceneLoaded;
        DontDestroyOnLoad(this.gameObject);
    }
    
    public void ShowLevelInfo(int levelIndex) {
        _actions.Player.Disable();
        _levelController.SetupLevel(levelIndex);
        _currentLevelSettings = _levelController.GetCurrentLevelSettings();
        _currentUiCanvas.ShowLevelInfo(_currentLevelSettings);
        EnableUICursor();
        
        ProgressionController.Reset();
        ProgressionController.Setup(_currentLevelSettings, _currentUiCanvas.UpgradePanel);
    }

    public void StartLevel() {
        _currentUiCanvas.StartLevel();
        _levelController.StartCurrentLevel();
        _actions.Player.Enable();
        DisableUICursor();
    }

    public void OpenEscMenu() {
        _actions.Player.Disable();
        Time.timeScale = 0f;
        EnableUICursor();
        _currentUiCanvas.ShowEscMenu();
    }

    public void OpenUpgradePanel() {
        _actions.Player.Disable();
        Time.timeScale = 0f;
        EnableUICursor();
        _currentUiCanvas.ShowUpgradePanel();
    }

    public void Resume() {
        _actions.Player.Enable();
        Time.timeScale = 1;
        DisableUICursor();
        _currentUiCanvas.HidePausePanels();
    }

    public void LoadMainMenu() {
        LoadScene(_mainMenuSceneName);
        _currentLevelSettings = null;
    }

    public void RestartCurrentLevel() {
        var sceneName = _levelController.GetCurrentLevelSettings().LevelSceneName;
        LoadScene(sceneName);
    }

    public void StartNextLevel() {
        var nextLevelSettings = _levelController.GetNextLevelSettings();
        if (nextLevelSettings == null) {
            LoadMainMenu();
            return;
        }
        var sceneName = nextLevelSettings.LevelSceneName;
        LoadScene(sceneName);
    }

    public void LoadTestPreset() {
        DisableUICursor();
        _currentLevelSettings = null;
        
        ProgressionController.Reset();
        ProgressionController.Setup(null, _currentUiCanvas.UpgradePanel);
    }

    public void ChangeMicrobe(Microbe microbe) {
        if (microbe == null) {
            _currentUiCanvas.PlayerPanel.HideUsableHint();
        }
        else {
            _currentUiCanvas.PlayerPanel.ShowUsableHint("E" , "Collect the microbes in a flask");
        }
    }

    public void UpdatePoints(int points) {
        _currentUiCanvas.PlayerPanel.UpdatePoints(points);
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

    private void LoadScene(string levelSceneName) {
        ProgressionController.Reset();
        SceneManager.LoadScene(levelSceneName);
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
        EnableUICursor();
        _currentUiCanvas.ShowGameOverPanel();
    }

    private void HandleGameWin() {
        _actions.Player.Disable();
        EnableUICursor();
        _currentUiCanvas.ShowWinLevelPanel();
    }

    private void HandleLevelProgressUpdated(int levelProgress) {
        var levelSettings = _levelController.GetCurrentLevelSettings();
        if (levelSettings == null) {
            return;
        }
        _currentUiCanvas.UpdateProgress(levelProgress, levelSettings.LevelGoal);
    }

    private void EnableUICursor() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void DisableUICursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleSceneLoaded(string sceneName) {
        _currentUiCanvas.gameObject.SetActive(sceneName != _mainMenuSceneName);
        for (int i = 0; i < levelSceneNames.Count; i++) {
            if (sceneName == levelSceneNames[i]) {
                ShowLevelInfo(i);
                return;
            }
        }

        if (sceneName != _mainMenuSceneName) {
            LoadTestPreset();
        }
    }
}
