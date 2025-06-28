using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DissolveObject))]
public class DestructiblePart : MonoBehaviour {
    public Action OnBeforeDestroy;
    
    [SerializeField] private DestructibleMaterialType _materialType;
    [SerializeField] private DestructiblePart _attachedTo;
    [SerializeField] [Min(0.1f)] private float _destroySpeedPerSecond = 1;

    private MeshFilter _meshFilter;
    private DissolveObject _dissolveObject;
    
    private bool _isBeingDestroyed = false;
    private float _currentDestructionRadius = 0;
    private Vector3 _destructionStartPointLcs;
    
    private List<DestructiblePart> _attachedParts = new();
    private FixedJoint _joint;

    public void RegisterAttachedPart(DestructiblePart part) {
        _attachedParts.Add(part);
    }

    public void StartDestruction(Vector3 destructionStartPointWcs) {
        _destructionStartPointLcs = transform.InverseTransformPoint(destructionStartPointWcs);
        _isBeingDestroyed = true;
        
        _dissolveObject.StartDestroy(destructionStartPointWcs);
    }

    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
        _dissolveObject = GetComponent<DissolveObject>();
    }

    private void Start() {
        if (!_attachedTo) {
            return;
        }
        
        _attachedTo.OnBeforeDestroy += HandleParentObjectDestroyed;
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = _attachedTo.GetComponent<Rigidbody>();
        _attachedTo.RegisterAttachedPart(this);
    }

    private void OnDestroy() {
        if (_attachedTo) {
            _attachedTo.OnBeforeDestroy -= HandleParentObjectDestroyed;
        }
        
        _attachedParts.Clear();
    }

    private void Update() {
        if (!_isBeingDestroyed) {
            return;
        }

        _currentDestructionRadius += Time.deltaTime * _destroySpeedPerSecond;
        _dissolveObject.SetDissolveRadius(_currentDestructionRadius);
        if (!AreBoundsInsideDestructionSphere()) {
            return;
        }

        DestroyPart();
        _isBeingDestroyed = false;
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

        if (_isBeingDestroyed) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(startPointWcs, 0.05f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPointWcs, _currentDestructionRadius);   
        }
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
        _dissolveObject.DestroyObject();
    }
}
