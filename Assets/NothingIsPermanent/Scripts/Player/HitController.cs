using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MicrobeController))]
[RequireComponent(typeof(BoxCollider))]
public class HitController : MonoBehaviour {
    [SerializeField] [Range(0, 180f)] private float _collectAngle = 20f;
    [SerializeField] private LayerMask layerMask;
    
    private MicrobeController _microbeController;

    private List<Microbe> _seenMicrobes = new();
    private InputSystem_Actions.PlayerActions _actions;
    private Microbe _currentUsableMicrobe;
    
    private void OnTriggerEnter(Collider other) {
        Microbe microbe = other.gameObject.GetComponent<Microbe>();
        if (!microbe) {
            microbe = other.gameObject.transform.parent?.GetComponent<Microbe>();
        }
        
        if (!microbe || _seenMicrobes.Contains(microbe)) {
            return;
        }

        _seenMicrobes.Add(microbe);
    }

    private void OnTriggerExit(Collider other) {
        Microbe microbe = other.gameObject.GetComponent<Microbe>();
        if (!microbe) {
            microbe = other.gameObject.transform.parent?.GetComponent<Microbe>();
        }
        
        if (!microbe || !_seenMicrobes.Contains(microbe)) {
            return;
        }

        _seenMicrobes.Remove(microbe);
    }

    void Start() {
        _actions = DIContainer.Inst.Actions.Player;
        _microbeController = GetComponent<MicrobeController>();
        DIContainer.Inst.ChangeMicrobe(_currentUsableMicrobe);

        _actions.Attack.started += TryStartDestroy;
        _actions.Interact.started += TryCollectMicrobe;
    }

    private void OnDestroy() {
        _actions.Attack.performed -= TryStartDestroy;
        _actions.Interact.performed -= TryCollectMicrobe;
    }

    void Update() {
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;

        float minAngle = 180;
        Microbe closestMicrobe = null;
        foreach (Microbe microbe in _seenMicrobes) {
            if (microbe.IsDestroyingNow || microbe.IsCollected) {
                continue;
            }
            
            Vector3 dirToMicrobe = (microbe.transform.position - cameraPos).normalized;
            float angle = Vector3.Angle(cameraForward, dirToMicrobe);
            
            if (angle > _collectAngle) {
                continue;
            }

            if (angle < minAngle) {
                minAngle = angle;
                closestMicrobe = microbe;
            }
        }

        if (_currentUsableMicrobe != closestMicrobe) {
            _currentUsableMicrobe = closestMicrobe;
            DIContainer.Inst.ChangeMicrobe(_currentUsableMicrobe);
        }
    }
    
    private void TryStartDestroy(InputAction.CallbackContext obj) {
        var cam = Camera.main;
        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask)) {
            DestructiblePart part = hit.collider.GetComponent<DestructiblePart>();
            if (part) {
                _microbeController.StartPartDestruction(part, hit.point);
                
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 3.0f);
                Debug.Log("Попал в: " + hit.collider.name);
            }
        }
        else {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 3.0f);
        }
    }
    
    private void TryCollectMicrobe(InputAction.CallbackContext obj) {
        if (!_currentUsableMicrobe) {
            return;
        }
        
        _microbeController.CollectMicrobe(_currentUsableMicrobe);
        _seenMicrobes.Remove(_currentUsableMicrobe);
    }
}
