using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DestructiblePart : MonoBehaviour {
    public Action OnBeforeDestroy;
    
    [SerializeField] private DestructibleMaterialType _materialType;
    [SerializeField] private DestructiblePart _attachedTo;
    [SerializeField] [Min(0.1f)] private float _health = 10f;
    [SerializeField] [Min(0.1f)] private float _destroySpeedPerSecond = 1;

    private bool _isBeingDestroyed = false;
    private FixedJoint _joint;

    [ContextMenu("Start")]
    private void StartDestroy() {
        _isBeingDestroyed = true;
    }
    
    [ContextMenu("Stop")]
    private void StopDestroy() {
        _isBeingDestroyed = false;        
    }

    private void Start() {
        if (!_attachedTo) {
            return;
        }
        
        _attachedTo.OnBeforeDestroy += HandleParentObjectDestroyed;
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = _attachedTo.GetComponent<Rigidbody>();
    }

    private void OnDestroy() {
        if (_attachedTo) {
            _attachedTo.OnBeforeDestroy -= HandleParentObjectDestroyed;
        }
    }

    private void Update() {
        if (!_isBeingDestroyed) {
            return;
        }

        _health -= Time.deltaTime * _destroySpeedPerSecond;
        if (_health <= 0) {
            DestroyPart();
        }
    }

    private void HandleParentObjectDestroyed() {
        _attachedTo = null;
        Destroy(_joint);
    }

    private void DestroyPart() {
        OnBeforeDestroy?.Invoke();
        Destroy(gameObject);
    }
}
