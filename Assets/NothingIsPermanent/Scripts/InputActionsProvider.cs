using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionsProvider : MonoBehaviour {
    public static InputActionsProvider Inst => _inst;
    public InputSystem_Actions Actions => _actions;
    
    private static InputActionsProvider _inst;
    private InputSystem_Actions _actions;
    
    private void Awake() {
        _actions = new();
        _actions.Enable();
        _inst = this;
    }
}
