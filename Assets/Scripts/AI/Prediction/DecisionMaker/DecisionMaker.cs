using System.Collections.Generic;

/// <summary>
/// 시뮬레이션 평가 결과를 기반으로 AI가 가장 유리한 액션을 선택하는 클래스
/// </summary>
public static class DecisionMaker
{
    // 여러 액션 결과 중 AI에게 가장 유리한 액션 선택
    public static int ChooseBestAction(IReadOnlyList<OutcomeEvaluation> evaluations)
    {
        int bestIndex = -1;
        float bestScore = float.NegativeInfinity;

        for (int i = 0; i < evaluations.Count; i++)
        {
            float score = CalculateAIScore(evaluations[i]);

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    // AI 관점에서 점수를 계산
    static float CalculateAIScore(in OutcomeEvaluation eval)
    {
        float score = 0f;

        // 플레이어 생존 방해
        score -= eval.SurvivalScore * 1.5f;

        // 탈출 방해
        score -= eval.EscapeScore * 2.0f;

        // 위험 유도는 AI에게 유리
        score += eval.DangerScore * 2.5f;

        // 테트리스 기여는 약하게 반영
        score -= eval.TetrisScore * 0.5f;

        return score;
    }
}