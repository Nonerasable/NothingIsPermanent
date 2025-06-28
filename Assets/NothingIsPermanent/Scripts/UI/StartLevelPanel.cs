using UnityEngine;
using UnityEngine.UI;

public class StartLevelPanel : MonoBehaviour {

    [SerializeField] TMPro.TextMeshProUGUI _levelDescriptionTextfield;
    [SerializeField] TMPro.TextMeshProUGUI _levelTimeTextfield;
    [SerializeField] TMPro.TextMeshProUGUI _levelGoalsTextfield;
    [SerializeField] Button _startLevelButton;

    private void Awake() {
        _startLevelButton.onClick.AddListener(StartLevel);
    }

    public void UpdateLevelInfo(LevelSettings levelSettings) {
        _levelDescriptionTextfield.text = levelSettings.StartLevelDescription;

        float time = levelSettings.LevelTime;
        var minutes = (int)(time / 60);
        var seconds = (int)(time % 60);
        _levelTimeTextfield.text = $"{minutes:D2}:{seconds:D2}";

        _levelGoalsTextfield.text = $"{levelSettings.LevelGoal} objects";
    }

    private void StartLevel() {
        DIContainer.Inst.StartLevel();
    }
}
