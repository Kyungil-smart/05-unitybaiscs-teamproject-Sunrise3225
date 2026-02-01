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

    public void MakeDropItem(Vector3 pos)
    {
        pos.y += 0.2f;

        if (_goldPrefab != null)
        {
            GameObject gold = Instantiate(_goldPrefab, pos, Quaternion.identity);
            AdjustDropY(gold, pos);
        }

        if (Random.value > _dropPercent)
            return;

        ItemType type = GetRandomItem();

        GameObject prefab = FindPrefabType(type);
        if (prefab == null)
            return; // 해당 타입 프리팹 없으면 그냥 스킵

        GameObject item = Instantiate(prefab, pos, Quaternion.identity);
        AdjustDropY(item, pos);
    }

    private void AdjustDropY(GameObject obj, Vector3 basePos)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col == null) return;

        basePos.y += col.bounds.extents.y;
        obj.transform.position = basePos;
    }

    private GameObject FindPrefabType(ItemType type)
    {
        if (_itemList == null) return null;

        for (int i = 0; i < _itemList.Length; i++)
        {
            GameObject prefab = _itemList[i];
            if (prefab == null) continue;

            // 프리팹에 붙어있는 아이템 스크립트에서 ItemType 프로퍼티 읽기
            AmmoItem ammo = prefab.GetComponent<AmmoItem>();
            if (ammo != null && ammo.ItemType == type) 
                return prefab;

            HealItem heal = prefab.GetComponent<HealItem>();
            if (heal != null && heal.ItemType == type) 
                return prefab;

            Freeze1SecItem freeze = prefab.GetComponent<Freeze1SecItem>();
            if (freeze != null && freeze.ItemType == type) 
                return prefab;

            FastMoveItem fast = prefab.GetComponent<FastMoveItem>();
            if (fast != null && fast.ItemType == type)
                return prefab;

            SlowMoveItem slow = prefab.GetComponent<SlowMoveItem>();
            if (slow != null && slow.ItemType == type)
                return prefab;
        }

        return null;
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