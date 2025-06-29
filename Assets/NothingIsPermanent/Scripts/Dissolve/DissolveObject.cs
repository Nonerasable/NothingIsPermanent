using System;
using System.Collections;
using UnityEngine;

public class DissolveObject : MonoBehaviour {
    public Action OnFinalDissolveThreshold;
    
    [SerializeField] Material dissolveMat;
    [SerializeField] Material dissolveFinishMat;

    public bool IsFullyDissolved => _isFullyDissolved;
    
    private Renderer _renderer;
    private bool _isFullyDissolved = false;
    private Color _destructionColor;

    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    private void Start() {
        _renderer.material = dissolveMat;
    }
    
    public void StartDestroy(Vector3 hitPoint, Color destructionColor) {
        _destructionColor = destructionColor;
        
        var mat = _renderer.material;
        mat.SetVector("_HitPoint", hitPoint);
        mat.SetFloat("_Radius", 0);
        mat.SetColor("_EdgeColor", _destructionColor);
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
        mat.SetColor("_EdgeColor", _destructionColor);
        float t = 0;

        bool isEventInvoked = false;

        float finalDissolveTime = DIContainer.Inst.MicrobeGlobalParams.FinalDissolveTIme;
        float finalDissolveThreshold = DIContainer.Inst.MicrobeGlobalParams.FinalDissolveThreshold;
        while (t < finalDissolveTime)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / finalDissolveTime);
            mat.SetFloat("_Progress", progress);

            if (!isEventInvoked && t > finalDissolveTime * finalDissolveThreshold) {
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
