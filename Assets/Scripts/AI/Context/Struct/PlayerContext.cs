using UnityEngine;

public readonly struct PlayerContext
{
    public readonly Vector2Int GridPosition;    // 현재 그리드 좌표
    public readonly int MoveDirection;          // 이동 방향
    public readonly bool IsStunned;             // 기절 상태

    public PlayerContext(Vector2Int gridPosition, int moveDirection, bool isStunned)
    {
        GridPosition = gridPosition;
        MoveDirection = moveDirection;
        IsStunned = isStunned;
    }
}