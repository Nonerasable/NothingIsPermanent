
using System;
using System.Collections.Generic;

public class MicrobeProgressionController {
    public float SpeedMultiplier = 1;

    private int _pendingSpeedUpgradeIdx;
    private List<int> _pendingMicrobeTypeUpgradeIdx = new();
    private int _currentPoints = 0;

    private UpgradePanel _upgradePanel;
    private LevelSettings _levelSettings;
    
    public MicrobeProgressionController() {
        foreach (var _ in Enum.GetValues(typeof(DestructibleMaterialType))) {
            _pendingMicrobeTypeUpgradeIdx.Add(0);
        }
    }

    public void Register(DestructibleObject dstrObject) {
        dstrObject.OnBeforeDestroy += HandleDestroyed;
        dstrObject.OnBeforePartDestroy += HandleDestroyed;
    }

    public void Reset() {
        if (_upgradePanel) {
            _upgradePanel.OnSpeedUpgrade -= UpgradeSpeed;
            _upgradePanel.OnMicrobeUpgrade -= UpgradeMicrobe;
        }
        
        SpeedMultiplier = 1;
        _pendingSpeedUpgradeIdx = 0;
        _currentPoints = 0;
        _levelSettings = null;
        
        for (int idx = 0; idx < _pendingMicrobeTypeUpgradeIdx.Count; idx++) {
            _pendingMicrobeTypeUpgradeIdx[idx] = 0;
        }
    }

    public void Setup(LevelSettings settings, UpgradePanel panel) {
        _levelSettings = settings;
        _upgradePanel = panel;

        _upgradePanel.OnSpeedUpgrade += UpgradeSpeed;
        _upgradePanel.OnMicrobeUpgrade += UpgradeMicrobe;
        
        UpdateUi();
    }
    
    private void HandleDestroyed(int points) {
        _currentPoints += points;
        
        UpdateUi();
    }

    private void UpgradeMicrobe(DestructibleMaterialType materialType) {
        foreach (MicrobeTypeUpgradeSettings typeUpgradeSettings in _levelSettings.MicrobeTypeUpgrade) {
            if (typeUpgradeSettings.microbeTypeToUpgrade != materialType) {
                continue;
            }

            int idx = (int)materialType;
            int pendingUpgradeIdx = _pendingMicrobeTypeUpgradeIdx[idx];
            _currentPoints -= typeUpgradeSettings.PriceProgression[pendingUpgradeIdx];
            if (pendingUpgradeIdx < typeUpgradeSettings.PriceProgression.Count - 1) {
                _pendingMicrobeTypeUpgradeIdx[idx] += 1;
            }
            
            DIContainer.Inst.Player.GetComponent<MicrobeController>().UpgradeMicrobe(materialType);
            
            UpdateUi();
            return;
        }
    }

    private void UpgradeSpeed() {
        MicrobeSpeedUpgradeSettings upgradeSettings = _levelSettings.MicrobeSpeedUpgrade[_pendingSpeedUpgradeIdx];
        
        _pendingSpeedUpgradeIdx += 1;
        _currentPoints -= upgradeSettings.Cost;
        SpeedMultiplier += upgradeSettings.AddedMultiplier;
        
        UpdateUi();        
    }

    public void UpdateUi() {
        DIContainer.Inst.UpdatePoints(_currentPoints);
        
        if (!_levelSettings) {
            _upgradePanel.SetSpeedButton(false, 0, 0, SpeedMultiplier);
            
            foreach (var type in Enum.GetValues(typeof(DestructibleMaterialType))) {
                _upgradePanel.SetTypeButton((DestructibleMaterialType)type, false, 0);
            }
            
            return;
        }
        
        int speedUpgradesCount = _levelSettings.MicrobeSpeedUpgrade.Count;
        bool canUpgradeSpeed = speedUpgradesCount != _pendingSpeedUpgradeIdx;
        if (canUpgradeSpeed) {
            MicrobeSpeedUpgradeSettings upgradeSettings = _levelSettings.MicrobeSpeedUpgrade[_pendingSpeedUpgradeIdx];

            bool canBuyUpgrade = _currentPoints >= upgradeSettings.Cost;

            if (canBuyUpgrade) {
                _upgradePanel.SetSpeedButton(true, upgradeSettings.AddedMultiplier, upgradeSettings.Cost, SpeedMultiplier);                            
            }
            else {
                _upgradePanel.SetSpeedButton(false, 0 ,0, SpeedMultiplier);
            }
        }
        else {
            _upgradePanel.SetSpeedButton(false, 0 ,0, SpeedMultiplier);
        }

        MicrobeController microbeController = DIContainer.Inst.Player?.GetComponent<MicrobeController>();
        
        for (int materialIdx = 0; materialIdx < _pendingMicrobeTypeUpgradeIdx.Count; ++materialIdx) {
            bool foundSettings = false;
            DestructibleMaterialType materialType = (DestructibleMaterialType)materialIdx;
            
            foreach (MicrobeTypeUpgradeSettings typeUpgradeSettings in _levelSettings.MicrobeTypeUpgrade) {
                if (typeUpgradeSettings.microbeTypeToUpgrade != materialType) {
                    continue;
                }
                
                int price = typeUpgradeSettings.PriceProgression[_pendingMicrobeTypeUpgradeIdx[materialIdx]];
                bool canBuy = _currentPoints >= price && (!microbeController || microbeController.HasMicrobesOfType(materialType));
                
                if (canBuy) {
                    _upgradePanel.SetTypeButton(typeUpgradeSettings.microbeTypeToUpgrade, true, price);
                }
                else {
                    _upgradePanel.SetTypeButton(typeUpgradeSettings.microbeTypeToUpgrade, false, 0);
                }

                foundSettings = true;
            }

            if (!foundSettings) {
                _upgradePanel.SetTypeButton(materialType, false, 0);
            }
        }
    }
}