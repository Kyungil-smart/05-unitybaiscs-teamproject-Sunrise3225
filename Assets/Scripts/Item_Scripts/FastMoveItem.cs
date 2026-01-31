using UnityEngine;
using static Define;

public class FastMoveItem : MonoBehaviour
{
    [Tooltip("이동속도 버프 수치를 넣어주세요.")]
    [SerializeField] private float _fastMoveVelue;
    [Tooltip("버프 지속시간을 넣어주세요.")]
    [SerializeField] private float _buffTime;

    [SerializeField] private ItemType itemType = ItemType.FastItem;
    private bool _isFastMove = false;


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    플레이어능력치 player = other.GetComponent<플레이어능력치>();

    //    if (player == null) return;

    //    if (!_isFastMove)
    //    {
    //        StartCoroutine(FastMove());
    //    }

    //    Destroy(gameObject);
    //}

    //private IEnumerator FastMove()
    //{
    //    _isFastMove = true;
    //    player.플레이어스피드 += _fastMoveVelue;
    //    yield return new WaitForSeconds(_buffTime);
    //    player.플레이어스피드 -= _fastMoveVelue;
    //    _isFastMove = false;
    //}
}
