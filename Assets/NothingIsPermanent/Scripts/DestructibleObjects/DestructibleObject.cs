using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestructibleObject : MonoBehaviour {
    public Action<int> OnBeforeDestroy;

    public Action<int> OnBeforePartDestroy;

    [SerializeField] [Min(0)] private int _defaultPointForPartDestruction = 0;
    [SerializeField] [Min(0)] private int _PointsForFullDestruction = 0;
    
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
            OnBeforeDestroy?.Invoke(_PointsForFullDestruction);
            Destroy(gameObject);
        }
    }
}
