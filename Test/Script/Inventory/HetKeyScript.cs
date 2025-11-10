using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HetKeyScript : MonoBehaviour
{
    public Image _icon;
    public TextMeshProUGUI _countText;
    int _index; // 슬롯 번호
    Button _button;

    public int Index { get { return _index; } set { _index = value; } }
    public Button Button { get { return _button; } set { _button = value; } }
}
