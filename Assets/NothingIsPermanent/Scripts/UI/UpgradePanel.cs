using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[Serializable]
public class UpgradeButtonSettings {
    public Button button;
    public DestructibleMaterialType upgradeFrom;
}

public class UpgradePanel : MonoBehaviour {
    
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _upgradeSpeedButton;
    [SerializeField] private List<UpgradeButtonSettings> _upgradeMicrobeTypeButtons;

    private void Awake() {
        _resumeButton.onClick.AddListener(Resume);
        _upgradeSpeedButton.onClick.AddListener(UpgradeMicrobeSpeed);
        foreach (var upgradeMicrobeTypeButton in _upgradeMicrobeTypeButtons) {
            upgradeMicrobeTypeButton.button.onClick.AddListener(() => UpgradeMicrobeType(upgradeMicrobeTypeButton.upgradeFrom));
        }
    }

    private void Resume() {
        DIContainer.Inst.Resume();
    }

    private void UpgradeMicrobeSpeed() {
        DIContainer.Inst.MicrobeGlobalParams.DestructionSpeed += 0.5f;
    }

    private void UpgradeMicrobeType(DestructibleMaterialType materialType) {
        DIContainer.Inst.Player.GetComponent<MicrobeController>().UpgradeMicrobe(materialType);
    }
}