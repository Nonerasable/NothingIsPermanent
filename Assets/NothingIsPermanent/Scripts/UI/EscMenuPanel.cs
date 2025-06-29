using UnityEngine;
using UnityEngine.UI;

public class EscMenuPanel : MonoBehaviour {
    
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _exitToMainMenuButton;

    private void Awake() {
        _resumeButton.onClick.AddListener(Resume);
        _exitToMainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void LoadMainMenu() {
        DIContainer.Inst.LoadMainMenu();
    }

    private void Resume() {
        DIContainer.Inst.Resume();
    }
}
