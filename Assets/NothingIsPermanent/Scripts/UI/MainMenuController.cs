using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _levelPanel;
    [SerializeField] private List<Button> _levelButtons;
    [SerializeField] private List<Button> _backButtons;

    [SerializeField] private string _firstLevelSceneName;
    [SerializeField] private string _secondLevelSceneName;
    [SerializeField] private string _thirdLevelSceneName;
    [SerializeField] private string _testLevelSceneName;
    
    private Stack<GameObject> _panelsStack = new Stack<GameObject>();
    private GameObject _currentPanel;
    
    private void Start() {
        for (int i = 0; i < _levelButtons.Count; i++) {
            var levelIndex = i;
            _levelButtons[i].onClick.AddListener(() => StartLevel(levelIndex));
        }
        foreach (var backButton in _backButtons) {
            backButton.onClick.AddListener(Back);
        }
        _playButton.onClick.AddListener(Play);
        _quitButton.onClick.AddListener(Quit);

        _currentPanel = _mainMenuPanel;
    }

    private void StartLevel(int levelIndex) {
        switch (levelIndex) {
            case 0: SceneManager.LoadScene(_firstLevelSceneName); break;
            case 1: SceneManager.LoadScene(_secondLevelSceneName); break;
            case 2: SceneManager.LoadScene(_thirdLevelSceneName); break;
            case 3: SceneManager.LoadScene(_testLevelSceneName); break;
        }
    }

    void Play() {
        OpenPanel(_levelPanel);
    }

    void OpenPanel(GameObject panel) {
        if (_currentPanel == panel) {
            return;
        }
        _currentPanel.gameObject.SetActive(false);
        _currentPanel = panel;
        _currentPanel.gameObject.SetActive(true);
        _panelsStack.Push(_currentPanel);
    }

    void Back() {
        if (_panelsStack.Count == 0) {
            return;
        }
        
        _currentPanel.gameObject.SetActive(false);
        _currentPanel = _panelsStack.Pop();
        _currentPanel.gameObject.SetActive(true);
    }

    void Quit() {
        Application.Quit();
    }
}
