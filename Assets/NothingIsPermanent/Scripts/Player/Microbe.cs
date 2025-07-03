using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

public class Microbe : MonoBehaviour {
    [SerializeField] [Min(0.1f)] private float _jumpingTime = 2f;
    [SerializeField] [Min(0.1f)] private float _jumpHeight = 0.3f;
    [SerializeField] [Min(0.1f)] private float _upwardsForceWhenJumpOff = 0.5f;

    [SerializeField] private List<GameObject> _microbeViews;

    public bool IsDestroyingNow => _currentPart;
    public bool IsCollected => _isCollected;
    public DestructibleMaterialType MaxAffectedMaterial => _maxAffectedMaterial;

    private Rigidbody _rigidBody;
    private Collider _collider;
    private MeshRenderer _renderer;
    private GameObject _currentView;
    private Color _microbeColor;
    
    private DestructibleMaterialType _maxAffectedMaterial;
    private bool _isCollected = false;
    
    private DestructiblePart _currentPart;
    
    private bool _isJumpingToNextPart = false;
    private float _jumpTimeCurrent = 0;
    private Vector3 _startJumpPosition;
    private Vector3 _targetJumpPositionLcs;

    private MicrobeProgressionController _progressionController;
    
    public void SetType(DestructibleMaterialType maxAffectedMaterial, Color color) {
        _maxAffectedMaterial = maxAffectedMaterial;
        _microbeColor = color;
        
        SelectView();
    }

    public void Collect() {
        if (_isCollected) {
            return;
        }

        _isCollected = true;
        _rigidBody.isKinematic = true;
        _collider.enabled = false;
        _renderer.enabled = false;
    }

    public void StartDestruction(DestructiblePart part, Vector3 destructionPoint, bool needJump = false) {
        Assert.IsNull(_currentPart, $"Already destructing part {_currentPart}, logic error");
        Assert.IsNotNull(part, "Trying to destroy null part");

        _isCollected = false;
        _renderer.enabled = true;
        
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

    private void OnEnable() {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Start() {
        _progressionController = DIContainer.Inst.ProgressionController;
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
            _jumpTimeCurrent += Time.deltaTime * _progressionController.SpeedMultiplier;
            float t = Mathf.Clamp01(_jumpTimeCurrent / _jumpingTime);
            Vector3 pos = GetParabolaPoint(_startJumpPosition, targetPointWcs, _jumpHeight, t);
            transform.position = pos;
            return;
        }

        _isJumpingToNextPart = false;
        StartDestructionInternal(targetPointWcs);
    }

    private void SelectView() {
        if (_currentView) {
            _collider.enabled = false;
            _renderer.enabled = false;
        }
        
        int idx = (int)_maxAffectedMaterial;
        
        _currentView = _microbeViews[idx];
        _collider = _currentView.GetComponent<Collider>();
        _renderer = _currentView.GetComponent<MeshRenderer>();
        
        _renderer.enabled = !_isCollected;
        _collider.enabled = !IsDestroyingNow && !_isCollected;
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
        
        _currentPart.StartDestruction(destructionPoint, _microbeColor);
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
            
            Quaternion rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-20f, 20f), Vector3.right);
            Quaternion rotation2 = Quaternion.AngleAxis(UnityEngine.Random.Range(-20f, 20f), Vector3.forward);
            Vector3 dir = rotation * Vector3.up;
            dir = rotation2 * dir;
            
            _rigidBody.AddForce(dir.normalized * _upwardsForceWhenJumpOff);
        }
    }
}
