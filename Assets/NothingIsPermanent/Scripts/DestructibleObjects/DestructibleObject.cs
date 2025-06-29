using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestructibleObject : MonoBehaviour {
    public Action OnBeforeDestroy;
    
    private List<DestructiblePart> _allParts = new();

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
            
            partToSub.SetDestructibleObject(this);
            part.OnBeforeDestroy += () => HandleRemovePart(partToSub);
        }
        DIContainer.Inst.LevelController.AddDestructibleObject(this);
    }

    private void HandleRemovePart(DestructiblePart part) {
        _allParts.Remove(part);

        if (_allParts.Count == 0) {
            OnBeforeDestroy?.Invoke();
            Destroy(gameObject);
        }
    }
}
