using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 이동 방향 + 시간 기반으로 미래 X 위치를 예측
/// </summary>
public class AIPrediction
{
    private const int MaxPredictDistance = 3;   // 과대 예측 방지
    private const float MinMoveThreshold = 0.2f;
    private const float BlockMoveSpeed = 3f;

    public List<int> PredictFutureXs(PlayerContext player, GridContext grid, int gridWidth)
    {
        List<int> result = new List<int>(4);

        int currentX = player.GridPosition.x;
        int currentY = player.GridPosition.y;

        // 1기절 상태
        if (player.IsStunned)
        {
            result.Add(currentX);
            return result;
        }

        int dir = player.MoveDirection;
        float moveSpeed = player.MoveSpeed;

        // 낙하 시간 계산 (Grid 기반)
        int dropY = grid.MaxHeight; // 현재 구조 기준 최선
        int dropDistance = Mathf.Abs(dropY - currentY);

        float timeToImpact = dropDistance / Mathf.Max(0.01f, BlockMoveSpeed);

        // 이동 거리 계산
        float moveAmount = moveSpeed * timeToImpact;

        int predictedMove = Mathf.RoundToInt(Mathf.Min(moveAmount, MaxPredictDistance));

        // 너무 작은 값 보정 
        if (predictedMove == 0 && moveAmount > MinMoveThreshold)
            predictedMove = 1;

        // 4정지 상태
        if (dir == 0)
        {
            AddUnique(result, currentX);

            AddUnique(result, Mathf.Clamp(currentX - 1, 0, gridWidth - 1));
            AddUnique(result, Mathf.Clamp(currentX + 1, 0, gridWidth - 1));

            return result;
        }

        // 이동 상태
        int forward = Mathf.Clamp(currentX + dir * predictedMove, 0, gridWidth - 1);
        int mid = Mathf.Clamp(currentX + dir * Mathf.Max(1, predictedMove / 2), 0, gridWidth - 1);
        int reverse = Mathf.Clamp(currentX - dir, 0, gridWidth - 1);

        AddUnique(result, forward);     // 주 이동 방향
        AddUnique(result, mid);         // 중간 위치
        AddUnique(result, currentX);    // 멈춤 가능성
        AddUnique(result, reverse);     // 반대 방향 (사람스러움)

        return result;
    }

    private void AddUnique(List<int> list, int value)
    {
        if (!list.Contains(value))
            list.Add(value);
    }
}