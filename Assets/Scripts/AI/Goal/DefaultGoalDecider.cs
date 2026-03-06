using UnityEngine;

/// <summary>
/// 기본 Goal 판단 구현체
/// 시뮬레이션 결과를 기반으로 현재 AI가 추구할 목적을 결정
/// </summary>
public class DefaultGoalDecider : IAIGoalDecider
{
    const float GoalSwitchMargin = 0.35f;

    EAIGoalType IAIGoalDecider.DecideGoal(in AISimulationState simulation, EAIGoalType currentGoal, float remainingLockTime, out float nextLockTime)
    {
        float survivalPressure = Inverse01(simulation.Score.SurvivalScore, 24f);
        float escapePressure = Inverse01(simulation.Score.EscapeScore, 6f);
        float dangerPressure = Mathf.Clamp01(simulation.Score.DangerScore / 4f);

        // 우선순위: 1) 죽일 수 있으면 KillNow
        if (CanKillNow(simulation, dangerPressure, survivalPressure, escapePressure))
        {
            nextLockTime = ResolveLockTime(EAIGoalType.KillNow, dangerPressure, escapePressure, survivalPressure);
            return EAIGoalType.KillNow;
        }
        if (CanTrap(simulation, dangerPressure, escapePressure))
        {
            nextLockTime = ResolveLockTime(EAIGoalType.TrapPlayer, dangerPressure, escapePressure, survivalPressure);
            return EAIGoalType.TrapPlayer;
        }

        // 3) Kill/Trap이 안되는 상황에서만 위협/실수유도(테트리스 방해/압박)
        float forceMistakeScore = 1.00f * dangerPressure + 0.80f * escapePressure;
        float pressureScore = 0.75f * survivalPressure + 0.55f * escapePressure + 0.40f * dangerPressure;

        // LockTime 반영: 기존 Goal의 Lock이 남아 있을수록 관성 보정을 주어 잦은 진동 방지
        if (currentGoal != EAIGoalType.None)
        {
            float baseLock = Mathf.Max(0.01f, AIGoalLockTime.GetLockTime(currentGoal));
            float lockRatio = Mathf.Clamp01(remainingLockTime / baseLock);
            float inertiaBonus = 0.25f * lockRatio;

            switch (currentGoal)
            {
                case EAIGoalType.ForceMistake:
                    forceMistakeScore += inertiaBonus;
                    break;
                case EAIGoalType.ApplyPressure:
                    pressureScore += inertiaBonus;
                    break;
            }
        }

        EAIGoalType bestGoal = EAIGoalType.ApplyPressure;
        float bestScore = pressureScore;

        if (forceMistakeScore > bestScore)
        {
            bestGoal = EAIGoalType.ForceMistake;
            bestScore = forceMistakeScore;
        }

        // 점수 차가 작으면 현재 Goal 유지 -> threshold machine 방지
        if (currentGoal != EAIGoalType.None)
        {
            float currentScore = GetScore(currentGoal, forceMistakeScore, pressureScore);
            if (bestGoal != currentGoal && bestScore - currentScore < GoalSwitchMargin)
                bestGoal = currentGoal;
        }

        nextLockTime = ResolveLockTime(bestGoal, dangerPressure, escapePressure, survivalPressure);
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

    static bool CanKillNow(in AISimulationState simulation, float dangerPressure, float survivalPressure, float escapePressure)
    {
        if (simulation.Score.SurvivalScore <= 0f)
            return true;

        return dangerPressure >= 0.60f && (survivalPressure >= 0.70f || escapePressure >= 0.75f);
    }

    static bool CanTrap(in AISimulationState simulation, float dangerPressure, float escapePressure)
    {
        if (simulation.Score.EscapeScore <= 0f)
            return true;

        return escapePressure >= 0.78f || (escapePressure >= 0.65f && dangerPressure >= 0.45f);
    }

    static float GetScore(EAIGoalType goal, float forceMistake, float pressure)
    {
        return goal switch
        {
            EAIGoalType.ForceMistake => forceMistake,
            EAIGoalType.ApplyPressure => pressure,
            _ => 0f,
        };
    }

    static float Inverse01(float value, float max)
    {
        return 1f - Mathf.Clamp01(value / Mathf.Max(0.01f, max));
    }
}