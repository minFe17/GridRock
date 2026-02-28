using UnityEngine;

/// <summary>
/// AI 의사결정 단계에서 사용하는 종합 시뮬레이션 상태
/// 예측 결과와 평가 결과를 하나로 묶어 Goal 선택 및 Action 판단의 기준으로 사용됨
/// </summary>
public readonly struct AISimulationState
{
    // --- 평가 점수 ---
    public readonly AIScore Score;

    // --- 위험/탈출 정보 ---
    public readonly AIThreat AIThreat;

    public readonly PlayerContext PlayerInfo;
    public readonly BlockState BlockInfo;

    public AISimulationState(in OutcomeEvaluation evaluation, AIThreat aIThreat, PlayerContext playerInfo, BlockState blockInfo)
    {
        Score = new AIScore(evaluation);
        AIThreat = aIThreat;
        PlayerInfo = playerInfo;
        BlockInfo = blockInfo;
    }
}