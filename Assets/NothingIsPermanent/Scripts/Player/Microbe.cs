using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Microbe : MonoBehaviour {
    [SerializeField] [Min(0.1f)] private float _jumpingTime = 2f;
    [SerializeField] [Min(0.1f)] private float _jumpHeight = 0.3f;
    [SerializeField] [Min(0.1f)] private float _upwardsForceWhenJumpOff = 0.5f;

    public bool IsDestroyingNow => _currentPart;

    private Rigidbody _rigidBody;
    private Collider _collider;
    
    private DestructibleMaterialType _maxAffectedMaterial;
    private float _destructionSpeed = 0.5f;
    
    private DestructiblePart _currentPart;
    
    private bool _isJumpingToNextPart = false;
    private float _jumpTimeCurrent = 0;
    private Vector3 _startJumpPosition;
    private Vector3 _targetJumpPositionLcs;
    
    public void Init(float destructionSpeed, DestructibleMaterialType maxAffectedMaterial) {
        _destructionSpeed = destructionSpeed;
        _maxAffectedMaterial = maxAffectedMaterial;
    }

    public void StartDestruction(DestructiblePart part, Vector3 destructionPoint, bool needJump = false) {
        Assert.IsNull(_currentPart, $"Already destructing part {_currentPart}, logic error");
        Assert.IsNotNull(part, "Trying to destroy null part");

        _currentPart = part;

        if (needJump) {
            part.SetWaitingForDestroy();
            _startJumpPosition = transform.position;
            _targetJumpPositionLcs = part.transform.InverseTransformPoint(destructionPoint);

            _isJumpingToNextPart = true;
            _jumpTimeCurrent = 0;
        }
        else {
            StartDestructionInternal(destructionPoint);
        }
    }

    private void Start() {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void OnDestroy() {
        if (_currentPart) {
            _currentPart.OnStartJumpOf -= HandlePartDestroyed;
        }
    }

    private void Update() {
        if (!_isJumpingToNextPart) {
            return;
        }

        Vector3 targetPointWcs = _currentPart.transform.TransformPoint(_targetJumpPositionLcs);
        
        if (_jumpTimeCurrent < _jumpingTime)
        {
            _jumpTimeCurrent += Time.deltaTime;
            float t = Mathf.Clamp01(_jumpTimeCurrent / _jumpingTime);
            Vector3 pos = GetParabolaPoint(_startJumpPosition, targetPointWcs, _jumpHeight, t);
            transform.position = pos;
            return;
        }

        _isJumpingToNextPart = false;
        StartDestructionInternal(targetPointWcs);
    }
    
    Vector3 GetParabolaPoint(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 mid = Vector3.Lerp(start, end, t);

        float parabola = 4 * height * t * (1 - t);
        mid.y = Mathf.Lerp(start.y, end.y, t) + parabola;

        return mid;
    }

    private void StartDestructionInternal(Vector3 destructionPoint) {
        transform.position = destructionPoint;
        transform.parent = _currentPart.transform;
        _rigidBody.isKinematic = true;
        _collider.enabled = false;
        
        _currentPart.StartDestruction(destructionPoint);
        _currentPart.OnStartJumpOf += HandlePartDestroyed;
    }

    private void HandlePartDestroyed() {
        transform.parent = null;
        _currentPart.OnStartJumpOf -= HandlePartDestroyed;

        DestructiblePart nextToDestroy = _currentPart.GetNextPartToDestroy();
        _currentPart = null;
        
        if (nextToDestroy) {
            StartDestruction(nextToDestroy, nextToDestroy.gameObject.transform.position, true);
        }
        else {
            _rigidBody.isKinematic = false;
            _collider.enabled = true;
            _rigidBody.linearVelocity = Vector3.up * _upwardsForceWhenJumpOff;
        }
    }
}
