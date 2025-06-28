using System;
using System.Collections;
using UnityEngine;

public class DissolveObject : MonoBehaviour
{
    [SerializeField] Material dissolveMat;
    [SerializeField] Material dissolveFinishMat;

    private Renderer _renderer;

    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    public void StartDestroy(Vector3 hitPoint) {
        var mat = _renderer.material;
        mat.SetVector("_HitPoint", hitPoint);
        mat.SetFloat("_Radius", 0);
    }

    public void SetDissolveRadius(float radius) {
        _renderer.material.SetFloat("_Radius", radius);
    }

    public void DestroyObject() {
        StartCoroutine(DissolveAndDestroy());
    }
    
    private IEnumerator DissolveAndDestroy() {
        _renderer.material = dissolveFinishMat;
        var mat = _renderer.material;
        float t = 0;
        const float time = 3f;
        while (t < time)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / time);
            mat.SetFloat("_Progress", progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}
