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
        StartCoroutine(ExpandDissolve(mat));
    }
    
    IEnumerator ExpandDissolve(Material mat)
    {
        float radius = 0;
        while (radius < 3.0f) // допустим, радиус максимум 3
        {
            radius += Time.deltaTime * 0.5f;
            mat.SetFloat("_Radius", radius);
            yield return null;
        }
        StartCoroutine(DissolveAndDestroy());
    }
    
    private IEnumerator DissolveAndDestroy() {
        _renderer.material = dissolveFinishMat;
        var mat = _renderer.material;
        float t = 0;
        while (t < 3.0f)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / 3.0f);
            mat.SetFloat("_Progress", progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}
