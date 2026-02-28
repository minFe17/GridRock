using UnityEngine;

/// <summary>
/// 시뮬레이션된 상태를 여러 판단 기준으로 수치화하여 
/// AI 의사결정에 사용할 평가 결과를 생성하는 평가기 클래스
/// </summary>
public static class OutcomeEvaluator
{
    // 시뮬레이션된 상태를 여러 관점(생존/탈출/보상/위험)으로 평가
    public static OutcomeEvaluation Evaluate(in PredictedWorldState state)
    {
        float survivalScore = EvaluateSurvival(state);  // 살아남을 가능성
        float escapeScore = EvaluateEscape(state);      // 빠져나갈 수 있는지
        float tetrisScore = EvaluateTetris(state);      // 테트리스 기여도
        float dangerScore = EvaluateDanger(state);      // 위험 노출도

        return new OutcomeEvaluation(survivalScore, escapeScore, tetrisScore, dangerScore);
    }

    // 생존 가능성 평가
    static float EvaluateSurvival(in PredictedWorldState state)
    {
        return Mathf.Clamp(state.Spatial.ReachableTileCount / (float)state.Spatial.RegionSize, 0f, 1f) * 5f;
    }

    // 탈출 관점 평가
    static float EvaluateEscape(in PredictedWorldState state)
    {
        float score = 0f;
        score -= state.Spatial.ChokePointCount * 0.5f;

        if (state.Spatial.EscapePathLength <= 2)
            score -= 3f;

        if (state.Spatial.ReachableTileCount <= 1)
            score -= 5f; // 완전 갇힘

        return score;
    }

    // 테트리스 완성에 도움이 되는 상태인지 평가
    static float EvaluateTetris(in PredictedWorldState state)
    {
        // 임시: 병목이 많으면 압박 유리하다고 가정
        return state.Spatial.ChokePointCount * 0.5f;
    }

    // 위험 노출 평가
    static float EvaluateDanger(in PredictedWorldState state)
    {
        return state.FutureTrapRisk * 5f + state.Spatial.ChokePointCount * 0.5f;
    }
}