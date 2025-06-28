using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _levelTimeTextfield;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _noTimeColor;
    
    public void UpdateTime(float time) {
        if (time < 20.0) {
            _levelTimeTextfield.color = _noTimeColor;
        }
        else {
            _levelTimeTextfield.color = _normalColor;
        }
        var minutes = (int)(time / 60);
        var seconds = (int)(time % 60);
        _levelTimeTextfield.text = $"{minutes:D2}:{seconds:D2}";
    }
}
