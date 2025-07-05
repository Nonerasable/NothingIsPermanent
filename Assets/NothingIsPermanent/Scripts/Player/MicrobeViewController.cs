using UnityEngine;

public class MicrobeViewController : MonoBehaviour {
    private Animator _animator;
    private Rigidbody _rb;
    private ParticleSystem _particleSystem;
    private Light _light;

    private bool _canUseAnimation = false;
    private bool _isAnimPlaying = false;
    
    public void Init(Rigidbody rigidbody, ParticleSystem ps, Light light) {
        _rb = rigidbody;
        _light = light;
        _particleSystem = ps;
    }

    public void SetCanUseAnimation(bool value) {
        if (!value) {
            _animator.SetBool("SearchAnim", false);
            _isAnimPlaying = false;
            _particleSystem?.Stop();
            if (_light) {
                _light.enabled = false;                
            }
        }

        _canUseAnimation = value;
    }
    
    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!_rb) {
            return;
        }

        if (!_canUseAnimation) {
            return;
        }

        if (_isAnimPlaying) {
            return;
        }

        if (_rb.linearVelocity.magnitude > .1f) {
            return;
        }

        _isAnimPlaying = true;
        _animator.SetBool("SearchAnim", true);

        _rb.isKinematic = true;
        _rb.gameObject.transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;
        
        _particleSystem.Play();
        _light.enabled = true;
    }
    
    
}
