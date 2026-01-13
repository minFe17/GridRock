using UnityEngine;

public readonly struct AIStateContext
{
    public readonly float Aggression;           // 공격성 (난이도 상승 요소)
    public readonly float Intelligence;         // 판단 정밀도 (예측 정확도)
    public readonly int FailedPredictionCount;  // 최근 예측 실패 횟수
    public readonly float Cooldown;             // 즉사 패턴 방지용 쿨타임
    public readonly Vector2Int LastAttackPos;   // 마지막 공격 위치

    public AIStateContext(float aggression, float intelligence, int failedPredictionCount, float cooldown, Vector2Int lastAttackPos)
    {
        Aggression = aggression;
        Intelligence = intelligence;
        FailedPredictionCount = failedPredictionCount;
        Cooldown = cooldown;
        LastAttackPos = lastAttackPos;
    }
}