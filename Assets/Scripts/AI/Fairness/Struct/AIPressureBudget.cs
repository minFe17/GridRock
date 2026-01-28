using UnityEngine;

/// <summary>
/// AI 압박 누적 예산
/// 과도한 방해 제한용
/// </summary>
public struct AIPressureBudget
{
    public float Current; // 현재 압박량
    public float Max;     // 최대 압박량

    public bool CanApply(float cost)
    {
        return Current + cost <= Max;
    }

    public void Consume(float cost)
    {
        Current += cost;
    }

    public void Recover(float amount)
    {
        Current = Mathf.Max(0f, Current - amount);
    }
}