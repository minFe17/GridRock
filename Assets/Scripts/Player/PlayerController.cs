using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerAnimationController _animationController;
    private EPlayerStatus _status = EPlayerStatus.Idle;
    private bool _isGround = true;
    // 캐릭터 뭐 선택했는지 여기서 처리해서 animator 바꿔줘야됨
    void Start()
    {
        _animationController = GetComponent<PlayerAnimationController>();
    }
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame && _isGround)
        {
            //_isGround = false;
            _status = EPlayerStatus.Jump;
            _animationController.Jump();
        }
        if(Keyboard.current.dKey.isPressed && _isGround)
        {
            _animationController.StartWalk();
        }
        else if(Keyboard.current.aKey.isPressed && _isGround)
        {
            _animationController.StartWalk(true);
        }
        else
        {
            _animationController.StopWalk();
        }    
    }
}
