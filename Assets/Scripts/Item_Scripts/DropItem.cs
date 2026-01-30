using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 이 스크립트를 컴포넌트한 오브젝트를 몬스터 프리펩의 자식 오브젝트로 등록
// 죽음 관련 함수 스크립트에서
// [SerializeField] private DropItem _dropItem;
// 필드로 선언
// 몬스터 죽음 관련 함수에(혹은 코드에) 
// _dropItem.MakeDropItem(this.transform);
// 메서드를
// Destroy();
// 전에 호출 하도록 하면 작동할 것으로 예상.

public class DropItem : MonoBehaviour
{
    [Tooltip("드랍될 아이템의 오브젝트를 등록해 주세요.")]
    [SerializeField] private GameObject[] _itemList;
    [Tooltip("아이템 드랍 확률을 조정해 주세요.")]
    [SerializeField][Range(0, 1)] private float _dropPercent;

    public void MakeDropItem()
    {
        if (Random.value > _dropPercent || _itemList.Length == 0) return;

        int index = Random.Range(0, _itemList.Length);
       
        Transform monsterTransform = transform.parent != null ? transform.parent : transform;
        Vector3 dropPos = monsterTransform.position;

        GameObject Item = Instantiate(_itemList[index], dropPos, Quaternion.identity);

        Collider collider = Item.GetComponent<Collider>();
        if (collider != null)
        {
            dropPos.y += collider.bounds.extents.y;
            Item.transform.position = dropPos;
        }
    }
}