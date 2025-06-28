using UnityEngine;
using UnityEngine.UI;

public class WinLevelPanel : MonoBehaviour {
    
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _nextLevelButton;

    private void Awake() {
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
        _nextLevelButton.onClick.AddListener(GoToNextLevel);
    }

    private void LoadMainMenu() {
        DIContainer.Inst.LoadMainMenu();
    }

    private void GoToNextLevel() {
        DIContainer.Inst.StartNextLevel();
    }
}
