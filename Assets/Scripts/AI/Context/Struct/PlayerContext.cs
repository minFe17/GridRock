using UnityEngine;

public readonly struct PlayerContext
{
    public readonly Vector2Int GridPosition;    // 현재 그리드 좌표
    public readonly Vector2 Velocity;           // 이동 방향/속도
    public readonly float TimeSinceLastMove;    // 정체 여부 판단용
    public readonly bool IsGrounded;            // 바닥 여부
    public readonly bool IsStunned;             // 기절 상태

    public PlayerContext(Vector2Int gridPosition, Vector2 velocity, bool isGrounded, bool isStunned, float timeSinceLastMove)
    {
        GridPosition = gridPosition;
        Velocity = velocity;
        IsGrounded = isGrounded;
        IsStunned = isStunned;
        TimeSinceLastMove = timeSinceLastMove;
    }
}