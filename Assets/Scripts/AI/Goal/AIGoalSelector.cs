/// <summary>
/// 예측 + 시뮬레이션 결과를 기반으로 AI의 다음 목적을 결정
/// </summary>
public static class AIGoalSelector
{
    public static EAIGoalType SelectGoal(in PredictionResult prediction, in OutcomeEvaluation eval)
    {
        // 즉시 사망 유도 가능
        if (eval.DangerScore >= 3.0f && eval.SurvivalScore <= 0f)
            return EAIGoalType.KillNow;

        // 갇힐 가능성 있음
        if (!prediction.HasEscapeRoute)
            return EAIGoalType.TrapPlayer;

        // 위험하지만 바로 죽진 않음 → 실수 유도
        if (prediction.HasDanger)
            return EAIGoalType.ForceMistake;

        // 기본은 압박 유지
        return EAIGoalType.ApplyPressure;
    }
}