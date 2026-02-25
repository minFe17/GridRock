using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class PlayerController : MonoBehaviour
{
    private const float STUN_TIME = 1f;
    private const float MOVE_SPEED = 3f;
    private const float JUMP_FORCE = 5f;

    private PlayerAnimationController _animationController;
    private EPlayerStatus _status = EPlayerStatus.Idle;
    private bool _isGround = true;
    private PlayerState _state = new PlayerState();
    private float _timer = 0f;

    private Rigidbody2D _rigid;
    private int _moveDirection = 0;

    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private float _groundCheckDistance = 0.2f;


    public PlayerState State
    {
        get { return _state; }
    }
    private void CheckGround()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.5f);

        _isGround = Physics2D.Raycast(
            origin,
            Vector2.down,
            _groundCheckDistance,
            _groundLayer
        );
        
    }

    private void Start()
    {
        _animationController = GetComponentInChildren<PlayerAnimationController>();
        _rigid = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        CheckGround();
        Move();

    }

    private void FixedUpdate()
    {
        _rigid.linearVelocity = new Vector2(_moveDirection * MOVE_SPEED, _rigid.linearVelocity.y);
    }

    private void Move()
    {
        if (_state.IsStun)
        {
            //PlayerContext playerContext = new PlayerContext(, 0, true);               // 그리드 좌표 필요(첫번쨰 매개변수)
            //SimpleSingleton<AIContextBuilder>.Instance.PlayerContext = playerContext;
            return;
        }

        _moveDirection = 0;

        
        if (Keyboard.current.dKey.isPressed)
        {
            Right();
            _moveDirection = 1;
        }
        else if (Keyboard.current.aKey.isPressed)
        {
             Left();
            _moveDirection = -1;
        }

        //PlayerContext playerContext = new PlayerContext(, _moveDirection, true);      // 그리드 좌표 필요(첫번쨰 매개변수)
        //SimpleSingleton<AIContextBuilder>.Instance.PlayerContext = playerContext;

        if (_isGround && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }

    private void Jump()
    {
        _status = EPlayerStatus.Jump;
        _animationController.Jump();
        _isGround = false; 

        _rigid.linearVelocity = new Vector2(_rigid.linearVelocity.x, JUMP_FORCE);
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
