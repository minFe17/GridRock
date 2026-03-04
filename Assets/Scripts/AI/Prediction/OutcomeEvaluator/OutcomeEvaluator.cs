/// <summary>
/// 시뮬레이션된 상태를 여러 판단 기준으로 수치화하여 
/// AI 의사결정에 사용할 평가 결과를 생성하는 평가기 클래스
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
        if (IsTrap(state))
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
        float survival = state.Spatial.ReachableTileCount;
        float escape = state.Spatial.ReachableTileCount;
        float danger = state.Spatial.AdjacentBlockCount;
        float tetris = 0f;

        // Goal별 total 계산
        float totalScore = CalculateTotalScore(goal, survival, escape, danger, tetris);

        return new OutcomeEvaluation(goal, survival, escape, danger, tetris, totalScore);
    }

    static float CalculateTotalScore(EAIGoalType goal, float survival, float escape, float danger, float tetris)
    {
        switch (goal)
        {
            case EAIGoalType.KillNow:
                return danger * 5f - escape * 3f;

            case EAIGoalType.TrapPlayer:
                return -escape * 4f + danger * 2f;

            case EAIGoalType.ForceMistake:
                return danger * 3f - survival * 1f;

            case EAIGoalType.ApplyPressure:
            default:
                return -escape * 2f + danger * 2f;
        }
    }

    static bool IsKill(in PredictedWorldState state)
    {
        return state.Spatial.ReachableTileCount == 0;
    }

    static bool IsTrap(in PredictedWorldState state)
    {
        return state.Spatial.ReachableTileCount <= 1;
    }

    static float EvaluatePressure(in PredictedWorldState state)
    {
        return -state.Spatial.ReachableTileCount + state.Spatial.AdjacentBlockCount * 3f;
    }

    static float EvaluateForceMistake(in PredictedWorldState state)
    {
        return state.Spatial.AdjacentBlockCount * 2f;
    }
}