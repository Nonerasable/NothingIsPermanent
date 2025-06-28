
using UnityEngine;
using UnityEngine.Assertions;

public class Microbe {
    private DestructibleMaterialType _maxAffectedMaterial;
    private float _destructionSpeed = 0.5f;
    
    private DestructiblePart _currentPart;

    public bool IsDestroyingNow => _currentPart; 
    
    public Microbe(float destructionSpeed, DestructibleMaterialType maxAffectedMaterial) {
        _destructionSpeed = destructionSpeed;
        _maxAffectedMaterial = maxAffectedMaterial;
    }
    
    ~Microbe()
    {
        if (_currentPart) {
            _currentPart.OnBeforeDestroy -= HandlePartDestroyed;
        }
    }

    public void StartDestruction(DestructiblePart part, Vector3 destructionPoint) {
        Assert.IsNull(_currentPart, $"Already destructing part {_currentPart}, logic error");

        _currentPart = part;
        part.StartDestruction(destructionPoint);
        part.OnBeforeDestroy += HandlePartDestroyed;
    }

    private void HandlePartDestroyed() {
        _currentPart.OnBeforeDestroy -= HandlePartDestroyed;

        DestructiblePart nextToDestroy = _currentPart.GetNextPartToDestroy();
        _currentPart = null;
        
        if (nextToDestroy) {
            StartDestruction(nextToDestroy, nextToDestroy.gameObject.transform.position);
        }
    }
}