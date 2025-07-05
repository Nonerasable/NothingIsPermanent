using System;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour {
    
    [SerializeField] StartLevelPanel _startLevelPanel;
    [SerializeField] WinLevelPanel _winLevelPanel;
    [SerializeField] GameOverPanel _gameOverPanel;
    [SerializeField] PlayerPanel _playerPanel;
    [SerializeField] EscMenuPanel _escPanel;
    [SerializeField] UpgradePanel _upgradePanel;
    
    public PlayerPanel PlayerPanel => _playerPanel;
    public UpgradePanel UpgradePanel => _upgradePanel;
    
    private void Awake() {
        _startLevelPanel.gameObject.SetActive(false);
        _winLevelPanel.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(false);
        _playerPanel.gameObject.SetActive(true);
        _escPanel.gameObject.SetActive(false);
        _upgradePanel.gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void ShowLevelInfo(LevelSettings levelSettings) {
        _startLevelPanel.gameObject.SetActive(true);
        _playerPanel.gameObject.SetActive(false);
        _escPanel.gameObject.SetActive(false);
        _startLevelPanel.UpdateLevelInfo(levelSettings);
    }

    public void StartLevel() {
        _startLevelPanel.gameObject.SetActive(false);
        _playerPanel.gameObject.SetActive(true);
    }

    public void UpdateTime(float time) {
        _playerPanel.UpdateTime(time);
    }

    public void UpdateProgress(int progress, int goal) {
        _playerPanel.UpdateLevelProgress(progress, goal);
    }

    public void ShowGameOverPanel() {
        _gameOverPanel.gameObject.SetActive(true);
    }

    public void ShowWinLevelPanel() {
        _winLevelPanel.gameObject.SetActive(true);
    }

    public void ShowEscMenu() {
        _escPanel.gameObject.SetActive(true);
    }

    public void ShowUpgradePanel() {
        _upgradePanel.gameObject.SetActive(true);
    }

    public void HidePausePanels() {
        _escPanel.gameObject.SetActive(false);
        _upgradePanel.gameObject.SetActive(false);
    }
}
