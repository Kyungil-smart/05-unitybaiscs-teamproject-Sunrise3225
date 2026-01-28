using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoveItem : MonoBehaviour
{
    [Tooltip("이동속도 디버프 수치를 넣어주세요.")]
    [SerializeField] private float _slowMoveVelue;
    [Tooltip("디버프 지속시간을 넣어주세요.")]
    [SerializeField] private float _debuffTime;
    private bool _isSlowMove = false;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    플레이어능력치 player = other.GetComponent<플레이어능력치>();

    //    if (player == null) return;

    //    if (!_isSlowMove)
    //    {
    //        StartCoroutine(SlowMove());
    //    }

    //    Destroy(gameObject);
    //}

    //private IEnumerator SlowMove()
    //{
    //    _isSlowMove = true;
    //    player.플레이어스피드 -= _slowMoveVelue;
    //    yield return new WaitForSeconds(_debuffTime);
    //    player.플레이어스피드 += _slowMoveVelue;
    //    _isSlowMove = false;
    //}
}
