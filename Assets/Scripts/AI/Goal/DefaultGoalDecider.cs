using UnityEngine;

/// <summary>
/// 기본 Goal 판단 구현체
/// 시뮬레이션 결과를 기반으로 현재 AI가 추구할 목적을 결정
/// </summary>
public class DefaultGoalDecider : IAIGoalDecider
{
    const float GoalSwitchMargin = 0.25f; // 점수 차이가 작으면 전환 허용
    readonly AIGoalWeightTable _goalWeights;

    public DefaultGoalDecider()
    {
        _goalWeights = AIGoalWeightTable.Shared;
    }

    public DefaultGoalDecider(AIGoalWeightTable goalWeights)
    {
        _goalWeights = goalWeights ?? AIGoalWeightTable.Shared;
    }

    EAIGoalType IAIGoalDecider.DecideGoal(in AISimulationState simulation, EAIGoalType currentGoal, float remainingLockTime, out float nextLockTime)
    {
        float survivalPressure = Inverse01(simulation.Score.SurvivalScore, 24f);
        float escapePressure = Inverse01(simulation.Score.EscapeScore, 6f);
        float dangerPressure = Mathf.Clamp01(simulation.Score.DangerScore / 4f);

        // Goal별 가중치
        float killWeight = _goalWeights.GetWeights(EAIGoalType.KillNow);
        float trapWeight = _goalWeights.GetWeights(EAIGoalType.TrapPlayer);
        float forceMistakeWeight = _goalWeights.GetWeights(EAIGoalType.ForceMistake);
        float pressureWeight = _goalWeights.GetWeights(EAIGoalType.ApplyPressure);

        // 점수 계산
        float killScore = (1.0f * dangerPressure + 0.5f * escapePressure + 0.25f * survivalPressure) * killWeight;
        float trapScore = (0.9f * escapePressure + 0.35f * dangerPressure) * trapWeight;
        float forceMistakeScore = (1.0f * dangerPressure + 0.8f * escapePressure) * forceMistakeWeight;
        float pressureScore = (0.9f * survivalPressure + 0.6f * escapePressure + 0.45f * dangerPressure) * pressureWeight;

        // 상황에 따른 추가 보너스
        if (CanKillNow(simulation, dangerPressure, survivalPressure, escapePressure, killWeight))
            killScore += 0.3f;
        if (CanTrap(simulation, dangerPressure, escapePressure))
            trapScore += 0.4f;

        // Goal 유지 보너스
        if (currentGoal != EAIGoalType.None)
        {
            float baseLock = Mathf.Max(0.01f, AIGoalLockTime.GetLockTime(currentGoal));
            float lockRatio = Mathf.Clamp01(remainingLockTime / baseLock);
            float inertiaBonus = 0.1f * lockRatio; // 너무 높으면 고착됨

            switch (currentGoal)
            {
                case EAIGoalType.KillNow: killScore += inertiaBonus; break;
                case EAIGoalType.TrapPlayer: trapScore += inertiaBonus; break;
                case EAIGoalType.ForceMistake: forceMistakeScore += inertiaBonus; break;
                case EAIGoalType.ApplyPressure: pressureScore += inertiaBonus; break;
            }
        }

        // Weighted Random 선택
        float total = killScore + trapScore + forceMistakeScore + pressureScore;
        float r = Random.value * total;

        EAIGoalType bestGoal;
        if (r < killScore) bestGoal = EAIGoalType.KillNow;
        else if (r < killScore + trapScore) bestGoal = EAIGoalType.TrapPlayer;
        else if (r < killScore + trapScore + forceMistakeScore) bestGoal = EAIGoalType.ForceMistake;
        else bestGoal = EAIGoalType.ApplyPressure;

        nextLockTime = ResolveLockTime(bestGoal, dangerPressure, escapePressure, survivalPressure);

        Debug.Log($"[Goal Random] Kill:{killScore:F2} Trap:{trapScore:F2} FM:{forceMistakeScore:F2} Pressure:{pressureScore:F2} → {bestGoal}");

        return bestGoal;
    }

    static float ResolveLockTime(EAIGoalType goal, float dangerPressure, float escapePressure, float survivalPressure)
    {
        float baseLockTime = AIGoalLockTime.GetLockTime(goal);
        return goal switch
        {
            EAIGoalType.KillNow => baseLockTime * Mathf.Lerp(0.85f, 1.20f, dangerPressure),
            EAIGoalType.TrapPlayer => baseLockTime * Mathf.Lerp(0.90f, 1.25f, escapePressure),
            EAIGoalType.ForceMistake => baseLockTime * Mathf.Lerp(0.90f, 1.15f, dangerPressure),
            EAIGoalType.ApplyPressure => baseLockTime * Mathf.Lerp(0.85f, 1.10f, survivalPressure),
            _ => 0f,
        };
    }

    static bool CanKillNow(in AISimulationState simulation, float dangerPressure, float survivalPressure, float escapePressure, float killWeight)
    {
        if (simulation.Score.SurvivalScore <= 0f)
            return true;

        return dangerPressure >= 0.75f && (survivalPressure >= 0.75f || escapePressure >= 0.8f);
    }

    static bool CanTrap(in AISimulationState simulation, float dangerPressure, float escapePressure)
    {
        if (simulation.Score.EscapeScore <= 0.05f) return true;
        if (escapePressure >= 0.65f && dangerPressure >= 0.4f) return true;
        return false;
    }

    static float Inverse01(float value, float max) => 1f - Mathf.Clamp01(value / Mathf.Max(0.01f, max));
}