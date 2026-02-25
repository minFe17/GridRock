using UnityEngine;

/// <summary>
/// AI의 내부 성향 및 동적 판단 상태를 담는 컨텍스트
/// </summary>
public readonly struct AIStateContext
{
    public readonly float Aggression;           // 공격성 (난이도 상승 요소)
    public readonly float Intelligence;         // 판단 정밀도 (예측 정확도)
    public readonly Vector2Int LastAttackPos;   // 마지막 공격 위치

    public AIStateContext(float aggression, float intelligence, Vector2Int lastAttackPos)
    {
        Aggression = aggression;
        Intelligence = intelligence;
        LastAttackPos = lastAttackPos;
    }
}