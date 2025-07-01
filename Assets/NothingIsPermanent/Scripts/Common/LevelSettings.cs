using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "Nothing Is Permanent/LevelSettings")]
public class LevelSettings : ScriptableObject {
    public int LevelTime;
    public int LevelGoal;
    public string StartLevelDescription;
    public string LevelSceneName;
    
    public List<MicrobeSettings> MicrobeSettings;
    public List<MicrobeSpeedUpgradeSettings> MicrobeSpeedUpgrade;
    public List<MicrobeTypeUpgradeSettings> MicrobeTypeUpgrade;
}

[Serializable]
public class MicrobeSettings {
    [Min(1)]
    public int Count = 1;
    public DestructibleMaterialType MaxAffectedMaterial;
}

[Serializable]
public class MicrobeSpeedUpgradeSettings {
    [Range(0f, 1f)] 
    public float AddedMultiplier;

    [Min(0)] public int Cost;
}

[Serializable]
public class MicrobeTypeUpgradeSettings {
    public DestructibleMaterialType microbeTypeToUpgrade;
    public List<int> PriceProgression;
}