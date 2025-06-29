using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour {
    
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _upgradeSpeedButton;
    [SerializeField] private List<Button> _upgradeMicrobeTypeButtons;

    private void Awake() {
        _resumeButton.onClick.AddListener(Resume);
        _upgradeSpeedButton.onClick.AddListener(UpgradeMicrobeSpeed);
        foreach (var upgradeMicrobeTypeButton in _upgradeMicrobeTypeButtons) {
            upgradeMicrobeTypeButton.onClick.AddListener(UpgradeMicrobeType);
        }
    }

    private void Resume() {
        DIContainer.Inst.Resume();
    }

    private void UpgradeMicrobeSpeed() {
    }

    private void UpgradeMicrobeType() {
        
    }
}