using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrobeController : MonoBehaviour {
    [SerializeField] private List<MicrobeSettings> _microbeSettings;
    [SerializeField] private GameObject _microbePrefab;
    [SerializeField] private GameObject _flask;
    
    private List<List<Microbe>> _microbesByMaterial = new();
    private List<GameObject> _microbes = new();

    private DestructibleMaterialType _currentMicrobeType = DestructibleMaterialType.WOOD;

    private Material _flaskMaterial;

    public void CollectMicrobe(Microbe microbe) {
        microbe.Collect();
        DIContainer.Inst.ChangeMicrobeCollection(microbe.MaxAffectedMaterial, _microbesByMaterial[(int)microbe.MaxAffectedMaterial]);
    }
    
    public void StartPartDestruction(DestructiblePart part, Vector3 startPoint) {
        if (part.IsBeingDestroyed) {
            return;
        }
        foreach (Microbe microbe in _microbesByMaterial[(int)_currentMicrobeType]) {
            if (!microbe.IsCollected) {
                continue;
            }
            
            microbe.StartDestruction(part, startPoint);
            DIContainer.Inst.ChangeMicrobeCollection(microbe.MaxAffectedMaterial, _microbesByMaterial[(int)microbe.MaxAffectedMaterial]);
            break;
        }
    }
    
    private void Start() {
        foreach (var _ in Enum.GetValues(typeof(DestructibleMaterialType))) {
            _microbesByMaterial.Add(new List<Microbe>());
        }
        
        foreach (MicrobeSettings settings in _microbeSettings) {
            for (int i = 0; i < settings.Count; i++) {
                GameObject microbe = Instantiate(_microbePrefab);
                _microbes.Add(microbe);
            
                Microbe microbeScript = microbe.GetComponent<Microbe>();
                microbeScript.Init(settings.destructionSpeed, settings.MaxAffectedMaterial);
                microbeScript.Collect();
            
                _microbesByMaterial[(int)settings.MaxAffectedMaterial].Add(microbeScript);
            }
        }
        
        MeshRenderer flaskRenderer = _flask.GetComponent<MeshRenderer>();
        foreach (Material material in flaskRenderer.materials) {
            if (material.name.Contains("LiquidFilled")) {
                _flaskMaterial = material;
                break;
            }
        }
        
        DIContainer.Inst.ChangeMicrobeCollection(_currentMicrobeType, _microbesByMaterial[(int)_currentMicrobeType]);
    }

    private void Update() {
        List<Microbe> currentMicrobeCollection = _microbesByMaterial[(int)_currentMicrobeType];

        int microbesCollected = 0;
        foreach (Microbe microbe in currentMicrobeCollection) {
            if (microbe.IsCollected) {
                microbesCollected += 1;
            }
        }
        
        _flaskMaterial.SetFloat("_Fill", (float)microbesCollected / currentMicrobeCollection.Count);
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
