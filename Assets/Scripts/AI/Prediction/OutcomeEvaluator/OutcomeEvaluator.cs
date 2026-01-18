/// <summary>
/// 시뮬레이션된 상태를 여러 판단 기준으로 수치화하여 
/// AI 의사결정에 사용할 평가 결과를 생성하는 평가기 클래스
/// </summary>
public static class OutcomeEvaluator
{
    // 시뮬레이션된 상태를 여러 관점(생존/탈출/보상/위험)으로 평가
    public static OutcomeEvaluation Evaluate(in SimulatedState state)
    {
        float survivalScore = EvaluateSurvival(state);  // 살아남을 가능성
        float escapeScore = EvaluateEscape(state);      // 빠져나갈 수 있는지
        float tetrisScore = EvaluateTetris(state);      // 테트리스 기여도
        float dangerScore = EvaluateDanger(state);      // 위험 노출도

        return new OutcomeEvaluation(survivalScore, escapeScore, tetrisScore, dangerScore);
    }

    // 생존 가능성 평가
    static float EvaluateSurvival(in SimulatedState state)
    {
        if (state.IsTrapped)
            return -5f;

        return state.HasEscapeRoute ? 3f : 0f;
    }

    // 탈출 관점 평가
    static float EvaluateEscape(in SimulatedState state)
    {
        return state.HasEscapeRoute ? 2.5f : -1.5f;
    }

    // 테트리스 완성에 도움이 되는 상태인지 평가
    static float EvaluateTetris(in SimulatedState state)
    {
        return state.HelpsTetris ? 2.0f : 0f;
    }

    // 위험 노출 평가
    static float EvaluateDanger(in SimulatedState state)
    {
        return state.NearDanger ? 3.0f : 0f;
    }
}