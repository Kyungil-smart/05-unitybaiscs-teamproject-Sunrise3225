using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // Blend Tree 파라미터 (Float)
    private readonly int _horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int _verticalHash   = Animator.StringToHash("Vertical");

    // 달리기 전이 파라미터 (Bool)
    private readonly int _isRunHash      = Animator.StringToHash("IsRun");

    // 점프 전이 파라미터 (Trigger)
    private readonly int _isJumpHash     = Animator.StringToHash("Jump");

    // 발사 전이 파라미터 (Trigger)
    private readonly int _fireHash       = Animator.StringToHash("Fire");

    // 재장전 전이 파라미터 (Trigger)
    private readonly int _reloadHash     = Animator.StringToHash("Reload");
    
    // 죽음 전이 파라미터 (Trigger)
    private readonly int _dieHash = Animator.StringToHash("Die");

    // 이동 판정 임계값
    private const float MoveThreshold = 0.01f;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateMoveBlendTree();
        UpdateRun();
        UpdateJump();
        UpdateFire();
        UpdateReload();
    }

    // 이동 입력을 2D Blend Tree 파라미터로 전달
    private void UpdateMoveBlendTree()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        _animator.SetFloat(_horizontalHash, horizontal);
        _animator.SetFloat(_verticalHash, vertical);
    }

    // 이동 중 Shift 입력 시 달리기 상태 전이
    private void UpdateRun()
    {
        float horizontal = _animator.GetFloat(_horizontalHash);
        float vertical   = _animator.GetFloat(_verticalHash);

        bool isMoving =
            (horizontal > MoveThreshold) || (horizontal < -MoveThreshold) ||
            (vertical   > MoveThreshold) || (vertical   < -MoveThreshold);

        bool shiftHeld =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        _animator.SetBool(_isRunHash, shiftHeld && isMoving);
    }

    // 점프 입력 순간 1회 트리거 발동
    private void UpdateJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger(_isJumpHash);
        }
    }

    // 단발 사격 입력 시 발사 트리거 발동
    private void UpdateFire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger(_fireHash);
        }
    }

    // 재장전 입력 순간 1회 트리거 발동
    private void UpdateReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _animator.SetTrigger(_reloadHash);
        }
    }
    
    // 죽음 트리거 발동 (로직은 CharacterController에서)
    public void TriggerDie()
    {
        _animator.SetTrigger(_dieHash);
    }

}
