using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {
    [SerializeField] TMPro.TextMeshProUGUI _levelProgressTextfield;
    [SerializeField] TMPro.TextMeshProUGUI _levelTimeTextfield;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _noTimeColor;
    [SerializeField] GameObject _usableHintContainer;
    [SerializeField] TMPro.TextMeshProUGUI _usableKeyTextfield;
    [SerializeField] TMPro.TextMeshProUGUI _usableHintTextfield;
    [SerializeField] TMPro.TextMeshProUGUI _microbeAmmoTextfield;
    [SerializeField] Image _microbeTypeImage;
    [SerializeField] Sprite _woodMicrobeImageTexture;
    [SerializeField] Sprite _metalMicrobeImageTexture;
    [SerializeField] Sprite _glassMicrobeImageTexture;
    
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

    public void UpdateLevelProgress(int levelProgress, int levelGoal) {
        _levelProgressTextfield.text = $"{levelProgress} / {levelGoal}";
    }

    public void ShowUsableHint(string keyName, string hintText) {
        _usableHintContainer.gameObject.SetActive(true);
        _usableKeyTextfield.text = keyName;
        _usableHintTextfield.text = hintText;
    }

    public void HideUsableHint() {
        _usableHintContainer.gameObject.SetActive(false);
    }

    public void UpdateMicrobeAmmo(int ammo, int maxAmmo, DestructibleMaterialType materialType) {
        _microbeAmmoTextfield.text = $"{ammo} / {maxAmmo}";
        switch (materialType) {
            case DestructibleMaterialType.WOOD: _microbeTypeImage.sprite = _woodMicrobeImageTexture; break;
            case DestructibleMaterialType.METAL: _microbeTypeImage.sprite = _metalMicrobeImageTexture; break;
            case DestructibleMaterialType.GLASS: _microbeTypeImage.sprite = _glassMicrobeImageTexture; break;
        }
    }
}
