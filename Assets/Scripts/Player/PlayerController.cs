using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const float STUN_TIME = 1f;
    private const float MOVE_SPEED = 0.5f;

    private PlayerAnimationController _animationController;
    private EPlayerStatus _status = EPlayerStatus.Idle;
    private bool _isGround = true;
    private PlayerState _state = new PlayerState();
    private float _timer = 0f;
    
    // 캐릭터 뭐 선택했는지 여기서 처리해서 animator 바꿔줘야됨

    public PlayerState State
    {
        get { return _state; }
    }
    void Start()
    {
        _animationController = GetComponentInChildren<PlayerAnimationController>();
    }
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_state.IsStun)
        {
            if (_timer >= STUN_TIME)
            {
                _timer = 0f;
                _state.IsStun = false;
            }
            _timer += Time.deltaTime;
            return;
        }

        if (!_isGround) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            //_isGround = false;
            //_status = EPlayerStatus.Jump;
            //_animationController.Jump();
            Jump();
        }
        if (Keyboard.current.dKey.isPressed && _isGround)
        {
            Right();
            Walk();
        }
        else if (Keyboard.current.aKey.isPressed && _isGround)
        {
            Left();
            Walk();
        }
        else
        {
            _animationController.StopWalk();
        }
    }

    private void Jump()
    {
        _status = EPlayerStatus.Jump;
        _animationController.Jump();
        //_isGround = false; => 이거 블럭이랑 충돌처리 확인해서 true바꿔줘야됨 (중력적용시켜야됨)
    }
    private void Walk()
    {
        if(_state.IsLeft)
        {
            transform.localPosition = new Vector3(transform.localPosition.x- MOVE_SPEED*Time.deltaTime, transform.localPosition.y);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x + MOVE_SPEED * Time.deltaTime, transform.localPosition.y);
        }
    }
    private void Left()
    {
        _animationController.Flip(true);
        _state.IsLeft = true;
    }
    private void Right()
    {
        _animationController.Flip();
        _state.IsRight = true;
    }
    private void Stun()
    {
        _animationController.Stun();
        _state.IsStun = true;
        _timer = 0f;
    }

}
