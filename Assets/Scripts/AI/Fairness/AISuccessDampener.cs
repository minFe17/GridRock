using UnityEngine;

/// <summary>
/// AI 연속 성공 감쇠
/// 과도한 압박 완화
/// </summary>
public class AISuccessDampener
{
    int _successStreak;    // 연속 성공 횟수

    public float GetModifier()
    {
        return Mathf.Clamp(1.0f - _successStreak * 0.15f, 0.4f, 1.0f);
    }

    public void OnSuccess()
    {
        _successStreak++;
    }

    public void OnFail()
    {
        _successStreak = Mathf.Max(0, _successStreak - 1);
    }
}