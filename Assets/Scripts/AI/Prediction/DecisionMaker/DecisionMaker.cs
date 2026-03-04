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
            float score = evaluations[i].TotalScore;

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }
        return bestIndex;
    }
}