using System;
using System.Collections;
using UnityEngine;

public class DissolveObject : MonoBehaviour
{
    [SerializeField] Material dissolveMat;
    [SerializeField] Material dissolveFinishMat;
    [SerializeField] [Min(0.01f)] private float _finalDissolveTime = 0.8f;

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
        while (t < _finalDissolveTime)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / _finalDissolveTime);
            mat.SetFloat("_Progress", progress);
            yield return null;
        }

        _isFullyDissolved = true;
    }
}
