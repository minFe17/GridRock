using UnityEngine;

/// <summary>
/// 플레이어의 현재 상황을 방향별로 정리해 예측 시스템에 전달하는 입력 데이터
/// </summary>
public readonly struct PredictionInput
{
    public readonly Vector2 PlayerPos;              // 현재 플레이어 위치 (예측 기준)

    public readonly EMoveDirectionType LastMoveDir; // 마지막 이동 방향
    public readonly float LastMoveDuration;         // 마지막 이동 유지 시간

    public readonly bool CanMoveLeft;               // 왼쪽 이동 가능 여부
    public readonly bool CanMoveRight;              // 오른쪽 이동 가능 여부

    public readonly bool CanEscapeLeft;             // 왼쪽 탈출 경로 존재
    public readonly bool CanEscapeRight;            // 오른쪽 탈출 경로 존재

    public readonly bool NearDangerLeft;            // 왼쪽 위험 인접 여부
    public readonly bool NearDangerRight;           // 오른쪽 위험 인접 여부

    public readonly bool NearTetrisLeft;            // 왼쪽 테트리스 기여 가능성
    public readonly bool NearTetrisRight;           // 오른쪽 테트리스 기여 가능성

    public PredictionInput(Vector2 playerPos, EMoveDirectionType lastMoveDir, float lastMoveDuration, bool canMoveLeft, bool canMoveRight, bool canEscapeLeft, bool canEscapeRight, bool nearDangerLeft, bool nearDangerRight, bool nearTetrisLeft, bool nearTetrisRight)
    {
        PlayerPos = playerPos;
        LastMoveDir = lastMoveDir;
        LastMoveDuration = lastMoveDuration;

        CanMoveLeft = canMoveLeft;
        CanMoveRight = canMoveRight;

        CanEscapeLeft = canEscapeLeft;
        CanEscapeRight = canEscapeRight;

        NearDangerLeft = nearDangerLeft;
        NearDangerRight = nearDangerRight;

        NearTetrisLeft = nearTetrisLeft;
        NearTetrisRight = nearTetrisRight;
    }
}