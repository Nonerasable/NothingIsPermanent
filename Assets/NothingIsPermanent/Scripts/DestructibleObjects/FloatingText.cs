using System;
using DG.Tweening;
using UnityEngine;

public class FloatingText : MonoBehaviour {

    [SerializeField] private TMPro.TextMeshProUGUI _textfield;
    [SerializeField] float moveUpDistance = 1f;
    [SerializeField] float duration = 1f;
    [SerializeField] Vector3 offset = new Vector3(0, 1f, 0);

    public Action<FloatingText> OnAnimationComplete;
    
    private Transform _cameraTransform;
    private Color _textColor;

    void Awake() {
        _cameraTransform = Camera.main.transform;
        _textColor = _textfield.color;
    }

    public void StartFloating() {
        transform.DOMove(transform.position + offset + Vector3.up * moveUpDistance, duration)
            .SetEase(Ease.OutCubic);

        _textfield.color = _textColor;
        _textfield.DOFade(0f, duration).SetEase(Ease.InCubic).OnComplete(() => OnAnimationComplete?.Invoke(this));
    }

    void LateUpdate() {
        transform.LookAt(_cameraTransform);
        transform.rotation = Quaternion.LookRotation(transform.position - _cameraTransform.position);
    }

    public void SetText(string message) {
        _textfield.text = message;
    }
}
