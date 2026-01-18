using UnityEngine;

/// <summary>
/// ActionSimulator에서 사용하는 시뮬레이션 중간/최종 상태 정보
/// 플레이어의 위치와 물리적 상태만 최소한으로 표현
/// </summary>
public readonly struct SimulationState
{
    public readonly Vector2Int GridPos;      // 현재 격자 위치
    public readonly bool IsGrounded;         // 지면에 붙어 있는가
    public readonly bool IsJumping;          // 점프 중인가
    public readonly int RemainingJump;       // 남은 점프 프레임 / 높이

    public SimulationState(Vector2Int gridPos, bool isGrounded, bool isJumping, int remainingJump)
    {
        GridPos = gridPos;
        IsGrounded = isGrounded;
        IsJumping = isJumping;
        RemainingJump = remainingJump;
    }
}