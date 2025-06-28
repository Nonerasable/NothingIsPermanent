using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Microbe : MonoBehaviour {
    [SerializeField] [Min(0.1f)] private float _jumpingTime = 2f;
    [SerializeField] [Min(0.1f)] private float _jumpHeight = 0.3f;
    
    private DestructibleMaterialType _maxAffectedMaterial;
    private float _destructionSpeed = 0.5f;
    
    private DestructiblePart _currentPart;
    
    private bool _isJumpingToNextPart = false;
    private float _jumpTimeCurrent = 0;
    private Vector3 _startJumpPosition;
    private Vector3 _targetJumpPositionLcs;

    public bool IsDestroyingNow => _currentPart; 
    
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
            transform.position = destructionPoint;
            part.StartDestruction(destructionPoint);
            part.OnStartJumpOf += HandlePartDestroyed;
        }
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
        
        if (_jumpTimeCurrent < _jumpingTime)
        {
            _jumpTimeCurrent += Time.deltaTime;
            float t = Mathf.Clamp01(_jumpTimeCurrent / _jumpingTime);
            Vector3 pos = GetParabolaPoint(_startJumpPosition, _currentPart.transform.TransformPoint(_targetJumpPositionLcs), _jumpHeight, t);
            transform.position = pos;
            return;
        }

        _isJumpingToNextPart = false;
        _currentPart.StartDestruction(_currentPart.transform.TransformPoint(_targetJumpPositionLcs));
        _currentPart.OnStartJumpOf += HandlePartDestroyed;
    }
    
    Vector3 GetParabolaPoint(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 mid = Vector3.Lerp(start, end, t);

        float parabola = 4 * height * t * (1 - t);
        mid.y = Mathf.Lerp(start.y, end.y, t) + parabola;

        return mid;
    }

    private void HandlePartDestroyed() {
        _currentPart.OnStartJumpOf -= HandlePartDestroyed;

        DestructiblePart nextToDestroy = _currentPart.GetNextPartToDestroy();
        _currentPart = null;
        
        if (nextToDestroy) {
            StartDestruction(nextToDestroy, nextToDestroy.gameObject.transform.position, true);
        }
    }
}
