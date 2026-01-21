using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator; //컨트롤러 캐릭터 선택한거랑, 게임모드에따라서 바꿔줘야됨

    public void ChangeCharacter(EPlayerAnimator playerAnimator)
    {
        switch (playerAnimator)
        {
            case EPlayerAnimator.PenguinNormal:
                //_animator.runtimeAnimatorController =  //애니메이터들 모아두자 한곳에 묶어서 로드해오자
                break;
            case EPlayerAnimator.DinoNormal:
                break;
        }
    }
    public void Jump()
    {
        _animator.SetTrigger("Jump");
    }
    public void StartWalk(bool isLeft = false)
    {
        Flip(isLeft);
        if (_animator.GetBool("IsWalk")) return;
        _animator.SetBool("IsWalk", true);
    }
    public void StopWalk()
    {
        if (!_animator.GetBool("IsWalk")) return;
        _animator.SetBool("IsWalk", false);
    }
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        
    }
    private void Flip(bool isLeft)
    {
        if (!isLeft)
            transform.localScale = new Vector3(1, 1, 1);
        else 
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
