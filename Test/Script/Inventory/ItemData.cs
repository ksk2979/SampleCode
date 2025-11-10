using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public int _id;
    public string _itemName;
    public Sprite _icon;
    public bool _stackable = true; // 스택을 쌓을수 있는 구조인가?
    public int _maxStack = 99; // 몇 스택까지 쌓게 할껀가

    [Header("World/Hand Prefab")]
    public GameObject _holdPrefab;
}
