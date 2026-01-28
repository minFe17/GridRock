using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 자주 탈출하는 방향 기록
/// 방해 방향 선택에 사용
/// </summary>
public class AIDirectionBias
{
    private readonly Dictionary<EMoveDirectionType, float> _bias = new()
    {
        { EMoveDirectionType.Left, 1.0f },
        { EMoveDirectionType.Right, 1.0f }
    };

    public float GetBias(EMoveDirectionType dir) => _bias[dir];

    public void Adjust(EMoveDirectionType dir, float delta)
    {
        _bias[dir] = Mathf.Clamp(_bias[dir] + delta, 0.2f, 3.0f);
    }
}