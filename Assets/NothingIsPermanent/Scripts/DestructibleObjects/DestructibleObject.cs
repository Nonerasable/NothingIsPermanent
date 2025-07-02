using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleObject : MonoBehaviour {
    public Action<int> OnBeforeDestroy;

    public Action<int> OnBeforePartDestroy;

    [SerializeField] [Min(0)] private int _defaultPointForPartDestruction = 0;
    [SerializeField] [Min(0)] private int _PointsForFullDestruction = 0;
    [SerializeField] private string _objectName;
    
    private List<DestructiblePart> _allParts = new();
    private DestructibleMaterialType _maxMaterialType;

    public DestructibleMaterialType MaxMaterialType => _maxMaterialType;

    public DestructiblePart GetNextPartToDestroy() {
        foreach (DestructiblePart part in _allParts) {
            if (part.IsBeingDestroyed) {
                continue;
            }

            return part;
        }

        return null;
    }
    
    private void Start() {
        _allParts = GetComponentsInChildren<DestructiblePart>().ToList();

        foreach (DestructiblePart part in _allParts) {
            DestructiblePart partToSub = part;
            if (part.MaterialType > _maxMaterialType) {
                _maxMaterialType = part.MaterialType;
            }
            
            partToSub.SetDestructibleObject(this);
            part.OnBeforeDestroy += () => HandleRemovePart(partToSub);
        }
        DIContainer.Inst.LevelController.AddDestructibleObject(this);
        DIContainer.Inst.ProgressionController.Register(this);
    }

    private void HandleRemovePart(DestructiblePart part) {
        OnBeforePartDestroy?.Invoke(part.PointsForDestrucion < 0 ? _defaultPointForPartDestruction : part.PointsForDestrucion);
        
        _allParts.Remove(part);

        if (_allParts.Count == 0) {
            ShowFloatingTextForFullObject(part);
            OnBeforeDestroy?.Invoke(_PointsForFullDestruction);
            Destroy(gameObject);
        }
        else {
            ShowFloatingTextForPart(part);
        }
    }

    private void ShowFloatingTextForPart(DestructiblePart part) {
        if (_allParts.Count == 0) {
            ShowFloatingTextForFullObject(part);
            return;
        }
        var floatingText = DIContainer.Inst.FloatingTextPool.GetFloatingText();
        floatingText.transform.position = part.transform.position;
        floatingText.SetText($"+{_defaultPointForPartDestruction} points");
        floatingText.StartFloating();
    }

    private void ShowFloatingTextForFullObject(DestructiblePart part) {
        var floatingText = DIContainer.Inst.FloatingTextPool.GetFloatingText();
        floatingText.transform.position = part.transform.position;
        floatingText.SetText($"{_objectName} is destroyed\n{_PointsForFullDestruction} points");
        floatingText.StartFloating();
    }
}
