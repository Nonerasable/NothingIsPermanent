using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrobeController : MonoBehaviour {
    [SerializeField] private List<MicrobeSettings> _microbeSettings;
    [SerializeField] private GameObject _microbePrefab;

    private List<List<Microbe>> _microbesByMaterial = new();
    private List<GameObject> _microbes = new();

    public void CollectMicrobe(Microbe microbe) {
        microbe.IsCollected = true;
    }
    
    public void StartPartDestruction(DestructiblePart part, Vector3 startPoint) {
        foreach (Microbe microbe in _microbesByMaterial[0]) {
            if (!microbe.IsCollected) {
                continue;
            }
            
            microbe.StartDestruction(part, startPoint);
            microbe.IsCollected = false;
        }
    }
    
    private void Start() {
        foreach (var _ in Enum.GetValues(typeof(DestructibleMaterialType))) {
            _microbesByMaterial.Add(new List<Microbe>());
        }
        
        foreach (MicrobeSettings settings in _microbeSettings) {
            GameObject microbe = Instantiate(_microbePrefab);
            _microbes.Add(microbe);
            
            Microbe microbeScript = microbe.GetComponent<Microbe>();
            microbeScript.Init(settings.destructionSpeed, settings.MaxAffectedMaterial);
            microbeScript.IsCollected = true;
            
            _microbesByMaterial[(int)settings.MaxAffectedMaterial].Add(microbeScript);
        }
    }

    private void OnDestroy() {
        foreach (GameObject microbe in _microbes) {
            Destroy(microbe);
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
