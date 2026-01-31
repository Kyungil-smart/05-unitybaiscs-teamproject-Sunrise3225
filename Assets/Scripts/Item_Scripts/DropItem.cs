using UnityEngine;
using static Define;

// 이 스크립트를 컴포넌트한 오브젝트를 몬스터 프리펩의 자식 오브젝트로 등록
// 죽음 관련 함수 스크립트에서
// [SerializeField] private DropItem _dropItem;
// 필드로 선언
// 몬스터 죽음 관련 함수에(혹은 코드에) 
// _dropItem.MakeDropItem();
// 메서드를
// Destroy();
// 전에 호출 하도록 하면 작동할 것으로 예상.

public class DropItem : MonoBehaviour
{
    [SerializeField] private GameObject _goldPrefab; // 돈은 무조건 드랍

    [Tooltip("드랍될 아이템의 오브젝트를 등록해 주세요.")]
    [SerializeField] private GameObject[] _itemList;
    [Tooltip("아이템 드랍 확률을 조정해 주세요.")]
    [SerializeField][Range(0, 1)] private float _dropPercent;
    private int _inDex;

    public void MakeDropItem(Vector3 pos)
    {
        pos.y += 0.2f;

        if (_goldPrefab != null)
            Instantiate(_goldPrefab, pos, Quaternion.identity);

        if (Random.value > _dropPercent)
            return;

        ItemType type = GetRandomItem();
        _inDex = (int)type;

        if (_itemList == null || _inDex < 0 || _inDex >= _itemList.Length)
            return;

        GameObject prefab = _itemList[_inDex];
        if (prefab == null) return;

        Instantiate(_itemList[_inDex], pos, Quaternion.identity);
    }
    public static ItemType GetRandomItem()
    {
        float randomValue = Random.value;
        float healChance = ITEM_DROP_PROB[(int)ItemType.HealItem];
        float ammoChance = ITEM_DROP_PROB[(int)ItemType.AmmoItem] + healChance;
        float fastChance = ITEM_DROP_PROB[(int)ItemType.FastItem] + ammoChance;
        float slowChance = ITEM_DROP_PROB[(int)ItemType.SlowItem] + fastChance;
        float freezeChance = ITEM_DROP_PROB[(int)ItemType.FreezeItem] + slowChance;

        if (randomValue < healChance)
            return ItemType.HealItem;
        else if (randomValue < ammoChance)
            return ItemType.AmmoItem;
        else if (randomValue < fastChance)
            return ItemType.FastItem;
        else if (randomValue < slowChance)
            return ItemType.SlowItem;
        else
            return ItemType.FreezeItem;
    }
}
