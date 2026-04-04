/// <summary>
/// 시뮬레이션된 상태를 기반으로 목표를 결정하고 점수화
/// </summary>
public static class OutcomeEvaluator
{
    public static OutcomeEvaluation Evaluate(in PredictedWorldState state)
    {
        EAIGoalType goal;

        // 즉사 가능
        if (state.SpatialAfter.ReachableTileCount <= 1)
            goal = EAIGoalType.KillNow;

        // 완전 고립 가능
        else if (IsTrap(state))
            goal = EAIGoalType.TrapPlayer;
        else
        {
            float pressureScore = EvaluatePressure(state);
            float mistakeScore = EvaluateForceMistake(state);

            goal = pressureScore >= mistakeScore ? EAIGoalType.ApplyPressure : EAIGoalType.ForceMistake;
        }

        // 기본 점수
        float survival = state.SpatialAfter.ReachableTileCount;
        float escape = state.SpatialAfter.EscapeRouteCount;
        float danger = state.SpatialAfter.DangerScore;
        float tetris = 0f;

        float reachableDelta = state.ReachableDelta;

        float totalScore = CalculateTotalScore(goal, survival, escape, danger, tetris, reachableDelta);

        float blockImpact = 0f;

        if (reachableDelta > 0)
            blockImpact = reachableDelta * 0.5f;

        totalScore += blockImpact;

        return new OutcomeEvaluation(goal, survival, escape, danger, tetris, totalScore);
    }

    static float CalculateTotalScore(EAIGoalType goal, float survival, float escape, float danger, float tetris, float reachableDelta)
    {
        switch (goal)
        {
            case EAIGoalType.KillNow:
                return danger * 5f - escape * 3f + reachableDelta * 4f;

            case EAIGoalType.TrapPlayer:
                return -escape * 4f + danger * 2f + reachableDelta * 3f;

            case EAIGoalType.ForceMistake:
                return danger * 3f - survival + reachableDelta * 2f;

            case EAIGoalType.ApplyPressure:
            default:
                return -escape * 2f + danger * 2f + reachableDelta * 2f;
        }
    }

    static bool IsTrap(in PredictedWorldState state)
    {
        return !state.SpatialAfter.HasEscapeRoute && state.SpatialAfter.ReachableTileCount <= 3;
    }

    static float EvaluatePressure(in PredictedWorldState state)
    {
        float escapePenalty = state.SpatialAfter.EscapeRouteCount == 0 ? 2f : 0f;
        return -state.SpatialAfter.ReachableTileCount + state.SpatialAfter.AdjacentBlockCount * 1.5f + state.ReachableDelta * 2f + escapePenalty;
    }

    static float EvaluateForceMistake(in PredictedWorldState state)
    {
        float cornerBonus = state.SpatialAfter.IsCornered ? 2f : 0f;
        return state.SpatialAfter.DangerScore * 2f + state.ReachableDelta + cornerBonus;
    }
}