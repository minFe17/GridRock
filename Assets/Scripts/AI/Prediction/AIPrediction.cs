using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 이동 방향을 기반으로 미래 X 위치 후보들을 예측하는 유틸리티 클래스
/// </summary>
public class AIPrediction
{
    public List<int> PredictFutureXs(PlayerContext player, int gridWidth)
    {
        List<int> result = new List<int>();

        int currentX = player.GridPosition.x;

        // 기절이면 현재 위치만
        if (player.IsStunned)
        {
            result.Add(currentX);
            return result;
        }

        int dir = player.MoveDirection;

        // 이동 안 하면 좌/우 둘 다 가능성
        if (dir == 0)
        {
            result.Add(currentX);
            result.Add(Mathf.Clamp(currentX - 1, 0, gridWidth - 1));
            result.Add(Mathf.Clamp(currentX + 1, 0, gridWidth - 1));
            return result;
        }

        // 기본 예측
        int predictDistance = 2;

        int forward = currentX + dir * predictDistance;
        int slightForward = currentX + dir * (predictDistance - 1);

        result.Add(Mathf.Clamp(forward, 0, gridWidth - 1));
        result.Add(Mathf.Clamp(slightForward, 0, gridWidth - 1));
        result.Add(currentX); // 멈출 가능성

        return result;
    }
}