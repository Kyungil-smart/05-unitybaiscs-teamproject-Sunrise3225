using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private readonly int _walkForwardHash = Animator.StringToHash("Walk_Forward");
    private readonly int _walkBackHash    = Animator.StringToHash("Walk_Back");
    private readonly int _walkLeftHash    = Animator.StringToHash("Walk_Left");
    private readonly int _walkRightHash   = Animator.StringToHash("Walk_Right");

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 임계값
        bool walkForward = vertical > 0.01f;
        bool walkBack    = vertical < -0.01f;
        bool walkRight   = horizontal > 0.01f;
        bool walkLeft    = horizontal < -0.01f;

        _animator.SetBool(_walkForwardHash, walkForward);
        _animator.SetBool(_walkBackHash, walkBack);
        _animator.SetBool(_walkLeftHash, walkLeft);
        _animator.SetBool(_walkRightHash, walkRight);
    }
}
