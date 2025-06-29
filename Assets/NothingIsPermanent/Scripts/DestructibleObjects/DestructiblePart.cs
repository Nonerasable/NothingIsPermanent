using System;
using System.Collections.Generic;
using UnityEngine;

enum DestructiblePartState {
    NONE,
    BEING_DESTROYED,
    FINAL_DISSOLVE,
    WAITING_FOR_DESTROY
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DissolveObject))]
public class DestructiblePart : MonoBehaviour {
    public Action OnBeforeDestroy;
    public Action OnStartJumpOf;
    
    [SerializeField] private DestructibleMaterialType _materialType;
    [SerializeField] private DestructiblePart _attachedTo;

    private MicrobeGlobalParams _microbeParams;
    
    private MeshFilter _meshFilter;
    private DissolveObject _dissolveObject;

    private DestructibleObject _destructibleObject;
    private DestructiblePartState state = DestructiblePartState.NONE;
    private float _currentDestructionRadius = 0;
    private Vector3 _destructionStartPointLcs;
    
    private List<DestructiblePart> _attachedParts = new();
    private FixedJoint _joint;

    public DestructibleMaterialType MaterialType => _materialType;
    public bool IsBeingDestroyed => state != DestructiblePartState.NONE;

    public DestructibleMaterialType GetMaxMaterialInWholeObject() {
        return _destructibleObject.MaxMaterialType;
    }
    
    public void SetDestructibleObject(DestructibleObject dstrObject) {
        _destructibleObject = dstrObject;
    }

    public void SetWaitingForDestroy() {
        state = DestructiblePartState.WAITING_FOR_DESTROY;
    }
    
    public DestructiblePart GetNextPartToDestroy() {
        if (_attachedTo && !IsBeingDestroyed) {
            return _attachedTo;
        }

        foreach (DestructiblePart attachedPart in _attachedParts) {
            if (attachedPart.IsBeingDestroyed) {
                continue;
            }

            return attachedPart;
        }

        return _destructibleObject.GetNextPartToDestroy();
    }
    
    public void RegisterAttachedPart(DestructiblePart part) {
        _attachedParts.Add(part);
    }

    public void StartDestruction(Vector3 destructionStartPointWcs, Color destructionColor) {
        if (state != DestructiblePartState.NONE && state != DestructiblePartState.WAITING_FOR_DESTROY) {
            return;
        }
        _destructionStartPointLcs = transform.InverseTransformPoint(destructionStartPointWcs);
        state = DestructiblePartState.BEING_DESTROYED;
        
        _dissolveObject.StartDestroy(destructionStartPointWcs, destructionColor);
    }

    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
        _dissolveObject = GetComponent<DissolveObject>();

        _dissolveObject.OnFinalDissolveThreshold += RaiseJumpOff;
    }

    private void Start() {
        _microbeParams = DIContainer.Inst.MicrobeGlobalParams;
        
        if (_attachedTo) {
            _attachedTo.OnBeforeDestroy += HandleParentObjectDestroyed;
            _joint = gameObject.AddComponent<FixedJoint>();
            _joint.connectedBody = _attachedTo.GetComponent<Rigidbody>();
            _attachedTo.RegisterAttachedPart(this);
        }
    }

    private void OnDestroy() {
        if (_attachedTo) {
            _attachedTo.OnBeforeDestroy -= HandleParentObjectDestroyed;
        }
        _attachedParts.Clear();

        _dissolveObject.OnFinalDissolveThreshold -= RaiseJumpOff;
    }

    private void Update() {
        switch (state) {
            case DestructiblePartState.NONE:
                return;
            case DestructiblePartState.BEING_DESTROYED:
                _currentDestructionRadius += Time.deltaTime * _microbeParams.DestructionSpeed;
                _dissolveObject.SetDissolveRadius(_currentDestructionRadius);
                if (!AreBoundsInsideDestructionSphere()) {
                    return;
                }

                state = DestructiblePartState.FINAL_DISSOLVE;
                _dissolveObject.StartFinalDissolve();
                break;
            case DestructiblePartState.FINAL_DISSOLVE:
                if (_dissolveObject.IsFullyDissolved) {
                    DestroyPart();
                }
                break;
            case DestructiblePartState.WAITING_FOR_DESTROY:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDrawGizmos() {
        if (!_meshFilter) {
            return;
        }
        
        Vector3 startPointWcs = transform.TransformPoint(_destructionStartPointLcs);
        
        foreach (Vector3 vertex in GetMeshBoundingVertices()) {
            if (Vector3.Distance(vertex, startPointWcs) > _currentDestructionRadius) {
                Gizmos.color = Color.red;
            }
            else {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawSphere(vertex, 0.05f);            
        }

        if (state == DestructiblePartState.BEING_DESTROYED) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(startPointWcs, 0.05f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPointWcs, _currentDestructionRadius);   
        }
    }

    private void RaiseJumpOff() {
        OnStartJumpOf?.Invoke();
    }

    private bool AreBoundsInsideDestructionSphere() {
        Vector3 startPointWcs = transform.TransformPoint(_destructionStartPointLcs);
        foreach (Vector3 vertex in GetMeshBoundingVertices()) {
            if (Vector3.Distance(vertex, startPointWcs) > _currentDestructionRadius) {
                return false;
            }
        }

        return true;
    }
    
    private List<Vector3> GetMeshBoundingVertices() {
        Bounds bounds = _meshFilter.mesh.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        List<Vector3> boundVertices = new() {
            //lower 
            transform.TransformPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z)),
            transform.TransformPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z)),
            transform.TransformPoint(new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z)),
            transform.TransformPoint(new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z)),
            //upper
            transform.TransformPoint(new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z)),
            transform.TransformPoint(new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z)),
            transform.TransformPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z)),
            transform.TransformPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z))
        };

        return boundVertices;
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
