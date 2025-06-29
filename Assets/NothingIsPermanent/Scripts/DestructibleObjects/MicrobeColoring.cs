using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MicrobeColorSettings 
{
    public DestructibleMaterialType material;
    public Color color;

}

[Serializable]
public class MicrobeColoring {
    [SerializeField] private List<MicrobeColorSettings> _colorSettings;

    public Color GetColor(DestructibleMaterialType materialType) {
        foreach (MicrobeColorSettings settings in _colorSettings) {
            if (settings.material == materialType) {
                return settings.color;
            }
        }
        
        return Color.black;
    }
}
