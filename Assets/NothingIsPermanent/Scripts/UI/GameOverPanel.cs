using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour {
    
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _tryAgainButton;

    private void Awake() {
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
        _tryAgainButton.onClick.AddListener(TryAgain);
    }

    private void LoadMainMenu() {
        DIContainer.Inst.LoadMainMenu();
    }

    private void TryAgain() {
        DIContainer.Inst.RestartCurrentLevel();
    }
}
