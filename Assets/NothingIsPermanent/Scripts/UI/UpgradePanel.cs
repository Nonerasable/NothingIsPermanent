using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[Serializable]
public class UpgradeButtonSettings {
    public Button button;
    public TextMeshProUGUI costTextField;
    public DestructibleMaterialType upgradeFrom;
}

public class UpgradePanel : MonoBehaviour {
    public Action OnSpeedUpgrade;
    public Action<DestructibleMaterialType> OnMicrobeUpgrade;
    
    [SerializeField] private TextMeshProUGUI _currentSpeedText;
    [SerializeField] private TextMeshProUGUI _upgradeText;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _upgradeSpeedButton;
    [SerializeField] private List<UpgradeButtonSettings> _upgradeMicrobeTypeButtons;

    public void SetSpeedButton(bool isAvailable, float multiplier, int price, float currenMultiplier) {
        _currentSpeedText.text = $"Current mul\nX{currenMultiplier}";
        if (!isAvailable) {
            _upgradeSpeedButton.enabled = false;
            _upgradeText.text = "";
            return;
        }

        _upgradeSpeedButton.enabled = true;
        _upgradeText.text = $"Buy + {multiplier * 100}%\n{price} points";
    }
    
    public void SetTypeButton(DestructibleMaterialType materialType, bool isAvailable, int price) {
        foreach (UpgradeButtonSettings upgradeButtonSettings in _upgradeMicrobeTypeButtons) {
            if (upgradeButtonSettings.upgradeFrom != materialType) {
                continue;
            }

            if (isAvailable) {
                upgradeButtonSettings.button.enabled = true;
                upgradeButtonSettings.costTextField.text = $"{price} points";
            }
            else {
                upgradeButtonSettings.button.enabled = false;
                upgradeButtonSettings.costTextField.text = "";
            }

            return;            
        }
    }
    
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
        OnSpeedUpgrade?.Invoke();
        // DIContainer.Inst.MicrobeGlobalParams.BaseDestructionSpeed += 0.5f;
    }

    private void UpgradeMicrobeType(DestructibleMaterialType materialType) {
        OnMicrobeUpgrade?.Invoke(materialType);
    }
}