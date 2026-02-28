using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 기본 AI 목적 종료 규칙 구현체
/// </summary>
public class DefaultGoalTermination : IAIGoalTermination
{
    public bool ShouldTerminate(EAIGoalType goal, in AISimulationState simulation)
    {
        switch (goal)
        {
            case EAIGoalType.KillNow:
                // 킬 유지 조건:
                // - 높은 위험
                // - 낮은 탈출 가능성
                // - 낮은 생존 여지
                // 위 조건이 깨지면 종료
                return simulation.Score.DangerScore < 2.5f || simulation.Score.EscapeScore > 0.5f || simulation.Score.SurvivalScore > 1.0f;


            case EAIGoalType.TrapPlayer:
                // 이미 완전히 갇힘
                return simulation.Score.EscapeScore <= 0.2f || simulation.Score.DangerScore >= 2.5f;

            case EAIGoalType.ForceMistake:
                // 충분히 위험 누적 -> KillNow 가능
                return simulation.Score.DangerScore >= 3.0f || simulation.Score.EscapeScore <= 0.3f;

            case EAIGoalType.ApplyPressure:
                // 기본 압박 단계
                // 위험이 일정 수준 이상 올라가면 상위 단계 전환
                return simulation.Score.DangerScore >= 2.0f;
        }

        return false;
    }
}