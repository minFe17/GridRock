/// <summary>
/// 기본 AI 목적 종료 규칙 구현체
/// </summary>
public class DefaultGoalTermination : IAIGoalTermination
{
    public bool ShouldTerminate(EAIGoalType goal, in PredictionResult prediction, in OutcomeEvaluation evaluation)
    {
        switch (goal)
        {
            case EAIGoalType.KillNow:
                // 실패했거나 더 이상 죽일 수 없음
                return evaluation.SurvivalScore > 0f;

            case EAIGoalType.TrapPlayer:
                // 이미 완전히 갇힘
                return !prediction.HasEscapeRoute;

            case EAIGoalType.ForceMistake:
                // 충분히 위험 누적 → KillNow 가능
                return evaluation.DangerScore >= 3.0f;

            case EAIGoalType.ApplyPressure:
                // 더 강한 목적 가능
                return prediction.HasDanger || !prediction.HasEscapeRoute;
        }

        return false;
    }
}