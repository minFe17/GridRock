/// <summary>
/// 기본 Goal 판단 구현체
/// 시뮬레이션 결과를 기반으로 현재 AI가 추구할 목적을 결정
/// </summary>
public class DefaultGoalDecider : IAIGoalDecider
{
    EAIGoalType IAIGoalDecider.DecideGoal(in AISimulationState simulation)
    {
        // 즉시 사망 유도 가능
        if (simulation.DangerScore >= 3.0f && simulation.SurvivalScore <= 0f)
            return EAIGoalType.KillNow;

        // 갇힐 가능성 있음
        if (!simulation.HasEscapeRoute)
            return EAIGoalType.TrapPlayer;

        // 위험하지만 바로 죽진 않음 → 실수 유도
        if (simulation.HasDanger)
            return EAIGoalType.ForceMistake;

        // 기본은 압박 유지
        return EAIGoalType.ApplyPressure;
    }
}