using UnityEngine;

public class DIContainer : MonoBehaviour
{
    [SerializeField] private PlayerCanvas _playerUICanvasPrefab;
    [SerializeField] private bool _isPlayerScene;
    
    public static DIContainer Inst => _inst;
    public InputSystem_Actions Actions => _actions;
    
    private static DIContainer _inst;
    private InputSystem_Actions _actions;
    private PlayerCanvas _currentUiCanvas;
    
    private void Awake() {
        _actions = new();
        _actions.Enable();
        _inst = this;

        if (_isPlayerScene) {
            _currentUiCanvas = Instantiate(_playerUICanvasPrefab);
        }
    }
}
