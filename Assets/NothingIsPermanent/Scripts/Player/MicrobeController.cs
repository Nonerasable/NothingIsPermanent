using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrobeController : MonoBehaviour {
    [SerializeField] private List<MicrobeSettings> _microbeSettings;

    private List<List<Microbe>> _microbesByMaterial = new();

    public void StartPartDestruction(DestructiblePart part, Vector3 startPoint) {
        foreach (Microbe microbe in _microbesByMaterial[0]) {
            if (microbe.IsDestroyingNow) {
                continue;
            }
            
            microbe.StartDestruction(part, startPoint);
        }
    }
    
    private void Start() {
        foreach (var _ in Enum.GetValues(typeof(DestructibleMaterialType))) {
            _microbesByMaterial.Add(new List<Microbe>());
        }
        
        foreach (MicrobeSettings settings in _microbeSettings) {
            Microbe microbe = new Microbe(settings.destructionSpeed, settings.MaxAffectedMaterial);
            _microbesByMaterial[(int)settings.MaxAffectedMaterial].Add(microbe);
        }
    }
}

[Serializable]
public class MicrobeSettings {
    [Min(1)]
    public int Count;
    public DestructibleMaterialType MaxAffectedMaterial;
    [Min(0.01f)] public float destructionSpeed = 1f;
}
