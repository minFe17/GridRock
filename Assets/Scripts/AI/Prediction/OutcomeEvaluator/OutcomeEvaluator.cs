/// <summary>
/// 시뮬레이션된 상태를 기반으로 목표를 결정하고 점수화
/// </summary>
public static class OutcomeEvaluator
{
    public static OutcomeEvaluation Evaluate(in PredictedWorldState state)
    {
        EAIGoalType goal;

        // 즉사 가능
        if (IsKill(state))
            goal = EAIGoalType.KillNow;

        // 완전 고립 가능
        else if (IsTrap(state))
            goal = EAIGoalType.TrapPlayer;
        else
        {
            // 공간 압박 점수
            float pressureScore = EvaluatePressure(state);

            // 실수 유도 점수 (선택지 축소 기반)
            float mistakeScore = EvaluateForceMistake(state);

            goal = pressureScore >= mistakeScore ? EAIGoalType.ApplyPressure : EAIGoalType.ForceMistake;
        }


        // 세부 점수 계산
        float survival = state.SpatialAfter.ReachableTileCount;
        float escape = state.SpatialAfter.ReachableTileCount;
        float danger = state.SpatialAfter.AdjacentBlockCount;
        float tetris = 0f;

        // Goal별 total 계산
        float reachableDelta = state.ReachableDelta;

        float totalScore = CalculateTotalScore(goal, survival, escape, danger, tetris, reachableDelta);

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
                return danger * 3f - survival * 1 + reachableDelta * 2f;

            case EAIGoalType.ApplyPressure:
            default:
                return -escape * 2f + danger * 2f + reachableDelta * 2f;
        }
    }

    static bool IsKill(in PredictedWorldState state)
    {
        return state.SpatialAfter.ReachableTileCount == 0;
    }

    static bool IsTrap(in PredictedWorldState state)
    {
        return state.SpatialAfter.ReachableTileCount <= 1;
    }

    static float EvaluatePressure(in PredictedWorldState state)
    {
        return -state.SpatialAfter.ReachableTileCount + state.SpatialAfter.AdjacentBlockCount * 3f + state.ReachableDelta * 2f;
    }

    static float EvaluateForceMistake(in PredictedWorldState state)
    {
        return state.SpatialAfter.AdjacentBlockCount * 2f + state.ReachableDelta;
    }
}