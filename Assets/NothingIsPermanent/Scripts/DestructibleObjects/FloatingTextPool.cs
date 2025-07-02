using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FloatingTextPool : MonoBehaviour  {
    
    [SerializeField] private FloatingText _floatingTextPrefab;
    
    private readonly List<FloatingText> _pool = new ();
    
    public FloatingText GetFloatingText() {
        FloatingText floatingText = null;
        if (_pool.Count == 0) {
            floatingText = Instantiate(_floatingTextPrefab);
            floatingText.OnAnimationComplete += AddTextfieldToPool;
        }
        else {
            floatingText = _pool[0];
            _pool.RemoveAt(0);
        }
        
        return floatingText;
    }

    private void AddTextfieldToPool(FloatingText floatingText) {
        _pool.Add(floatingText);
    }
}
