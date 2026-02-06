using UnityEngine;

/// <summary>
/// AI 압박 누적 예산
/// 과도한 방해를 제한하기 위한 페어니스 시스템 상태
/// </summary>
public class AIPressureBudget
{
    public float Current => current; // 읽기 전용
    public float Max => max;         // 읽기 전용

    float current;   // 내부 상태
    float max;       // 최대 허용치

    public AIPressureBudget(float max)
    {
        this.max = max;
        current = 0f;
    }

    public bool CanApply(float cost)
    {
        return current + cost <= max;
    }

    public void Consume(float cost)
    {
        current = Mathf.Min(max, current + cost);
    }

    public void Recover(float amount)
    {
        current = Mathf.Max(0f, current - amount);
    }
}