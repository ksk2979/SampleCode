using TMPro;
using UnityEngine;

// 단순하게 에임을 껏다켰다 하고 상호작용 되는 물체에 확인 UI나오게 하는것
public class AimScript : MonoBehaviour
{
    [SerializeField] GameObject _aimObj;
    [SerializeField] TextMeshProUGUI _text;

    // 인벤 활성화나 esc 메뉴 열때에 에임 없어져야함 (아니면 뒤로 가려지게 해도 되긴하는데.. 흠)
    public void AimActive(bool active)
    {
        _aimObj.SetActive(active);
    }
    
    public void TextActive(bool active)
    {
        _text.gameObject.SetActive(active);
    }
    public void TextStrSetting(string text)
    {
        _text.text = $"{text}";
    }

    public bool CheckAimActive { get { return _aimObj.activeSelf; } }
    public bool CheckTextActive { get { return _text.gameObject.activeSelf; } }
}
