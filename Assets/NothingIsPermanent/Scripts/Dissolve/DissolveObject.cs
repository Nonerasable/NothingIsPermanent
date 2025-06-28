using System;
using System.Collections;
using UnityEngine;

public class DissolveObject : MonoBehaviour {
    public Action OnFinalDissolveThreshold;
    
    [SerializeField] Material dissolveMat;
    [SerializeField] Material dissolveFinishMat;
    [SerializeField] [Min(0.01f)] private float _finalDissolveTime = 0.8f;
    [SerializeField] [Range(0.0f, 1.0f)] private float _finalDissolveThreshold = 0.8f;

    public bool IsFullyDissolved => _isFullyDissolved;
    
    private Renderer _renderer;
    private bool _isFullyDissolved = false;

    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    private void Start() {
        _renderer.material = dissolveMat;
    }
    
    public void StartDestroy(Vector3 hitPoint) {
        var mat = _renderer.material;
        mat.SetVector("_HitPoint", hitPoint);
        mat.SetFloat("_Radius", 0);
    }

    public void SetDissolveRadius(float radius) {
        _renderer.material.SetFloat("_Radius", radius);
    }

    public void StartFinalDissolve() {
        StartCoroutine(DissolveAndDestroy());
    }
    
    private IEnumerator DissolveAndDestroy() {
        _renderer.material = dissolveFinishMat;
        var mat = _renderer.material;
        float t = 0;

        bool isEventInvoked = false;
        
        while (t < _finalDissolveTime)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / _finalDissolveTime);
            mat.SetFloat("_Progress", progress);

            if (!isEventInvoked && t > _finalDissolveTime * _finalDissolveThreshold) {
                OnFinalDissolveThreshold?.Invoke();
                isEventInvoked = true;
            }
            
            yield return null;
        }

        if (!isEventInvoked) {
            OnFinalDissolveThreshold?.Invoke();
        }
        _isFullyDissolved = true;
    }
}
