using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MicrobeController : MonoBehaviour {
    [SerializeField] private List<MicrobeSettings> _microbeSettings;
    [SerializeField] private GameObject _microbePrefab;
    [SerializeField] private GameObject _flask;
    [SerializeField] private Transform _flaskTip;
    
    private List<List<Microbe>> _microbesByMaterial = new();
    private List<GameObject> _microbes = new();

    private DestructibleMaterialType _currentMicrobeType = DestructibleMaterialType.WOOD;

    private Material _flaskMaterial;
    private MicrobeColoring _coloring;

    private InputSystem_Actions.PlayerActions _actions;

    public void SetupMicrobe(List<MicrobeSettings> microbeSettings) {
        List<MicrobeSettings> settingsToUse = microbeSettings ?? _microbeSettings;
        
        foreach (MicrobeSettings settings in settingsToUse) {
            for (int i = 0; i < settings.Count; i++) {
                GameObject microbe = Instantiate(_microbePrefab);
                _microbes.Add(microbe);
            
                Microbe microbeScript = microbe.GetComponent<Microbe>();
                microbeScript.SetType(settings.MaxAffectedMaterial, _coloring.GetColor(settings.MaxAffectedMaterial));
                microbeScript.Collect();
            
                _microbesByMaterial[(int)settings.MaxAffectedMaterial].Add(microbeScript);
            }
        }
        
        SelectMicrobe(DestructibleMaterialType.WOOD);
    }

    public bool HasMicrobesOfType(DestructibleMaterialType type) {
        return _microbesByMaterial[(int)type].Count != 0;
    }
    
    public void CollectMicrobe(Microbe microbe) {
        microbe.Collect();

        TryUpdateUi(microbe.MaxAffectedMaterial);
    }

    public void UpgradeMicrobe(DestructibleMaterialType typeToUpgrade) {
        int idx = (int)typeToUpgrade;
        if (_microbesByMaterial[idx].Count == 0) {
            return;
        }
        
        if (Enum.GetValues(typeof(DestructibleMaterialType)).Length == idx) {
            return;
        }
        
        DestructibleMaterialType nextType = (DestructibleMaterialType)(idx + 1);
        
        // try to upgrade microbe in flask first
        // if all microbes are in use, upgrade first microbe
        Microbe microbeToUpgrade = _microbesByMaterial[idx][0];
        foreach (Microbe microbe in _microbesByMaterial[idx]) {
            if (microbe.IsDestroyingNow || !microbe.IsCollected) {
                continue;
            }

            microbeToUpgrade = microbe;
            break;
        }

        _microbesByMaterial[idx].Remove(microbeToUpgrade);
        
        microbeToUpgrade.SetType(nextType, _coloring.GetColor(nextType));
        _microbesByMaterial[(int)nextType].Add(microbeToUpgrade);

        TryUpdateUi(typeToUpgrade);
    }
    
    public void StartPartDestruction(DestructiblePart part, Vector3 startPoint) {
        if (part.IsBeingDestroyed) {
            return;
        }

        if (part.GetMaxMaterialInWholeObject() > _currentMicrobeType) {
            return;
        }
        
        foreach (Microbe microbe in _microbesByMaterial[(int)_currentMicrobeType]) {
            if (!microbe.IsCollected) {
                continue;
            }

            microbe.transform.position = _flaskTip.position;
            microbe.StartDestruction(part, startPoint, true);
            DIContainer.Inst.ChangeMicrobeCollection(microbe.MaxAffectedMaterial, _microbesByMaterial[(int)microbe.MaxAffectedMaterial]);
            break;
        }
    }

    private void SelectMicrobe(DestructibleMaterialType type) {
        _currentMicrobeType = type;
        _flaskMaterial.SetColor("_Color", _coloring.GetColor(type));
        DIContainer.Inst.ChangeMicrobeCollection(type, _microbesByMaterial[(int)type]);
    }

    private void TryUpdateUi(DestructibleMaterialType type) {
        if (type == _currentMicrobeType) {
            DIContainer.Inst.ChangeMicrobeCollection(type, _microbesByMaterial[(int)type]);            
        }
    }

    private void Awake() {
        foreach (var _ in Enum.GetValues(typeof(DestructibleMaterialType))) {
            _microbesByMaterial.Add(new List<Microbe>());
        }
        
        MeshRenderer flaskRenderer = _flask.GetComponent<MeshRenderer>();
        foreach (Material material in flaskRenderer.materials) {
            if (material.name.Contains("LiquidFilled")) {
                _flaskMaterial = material;
                break;
            }
        }
    }
    
    private void Start() {
        _actions = DIContainer.Inst.Actions.Player;
        _coloring = DIContainer.Inst.microbeColoring;
        
        _actions.SelectMicrobe1.started += HandleWoodMicrobeSelected;
        _actions.SelectMicrobe2.started += HandleMetalMicrobeSelected;
        _actions.SelectMicrobe3.started += HandleGlassMicrobeSelected;
    }

    private void HandleWoodMicrobeSelected(InputAction.CallbackContext obj) {
        SelectMicrobe(DestructibleMaterialType.WOOD);
    }
    
    private void HandleMetalMicrobeSelected(InputAction.CallbackContext obj) {
        SelectMicrobe(DestructibleMaterialType.METAL);
    }    
    private void HandleGlassMicrobeSelected(InputAction.CallbackContext obj) {
        SelectMicrobe(DestructibleMaterialType.GLASS);
    }

    private void Update() {
        List<Microbe> currentMicrobeCollection = _microbesByMaterial[(int)_currentMicrobeType];

        int microbesCollected = 0;
        foreach (Microbe microbe in currentMicrobeCollection) {
            if (microbe.IsCollected) {
                microbesCollected += 1;
            }
        }
        
        _flaskMaterial.SetFloat("_Fill", currentMicrobeCollection.Count == 0 ? 0 : (float)microbesCollected / currentMicrobeCollection.Count);
    }

    private void OnDestroy() {
        foreach (GameObject microbe in _microbes) {
            Destroy(microbe);
        }
    }
}
