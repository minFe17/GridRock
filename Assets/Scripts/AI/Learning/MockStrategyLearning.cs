using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 전략 결과를 기록하고, 성공률 기반 점수 보정값을 제공하는 Mock 학습 시스템
/// </summary>
public class MockStrategyLearning : IAIStrategyLearning
{
    const float SuccessWeightDelta = 0.12f;
    const float FailureWeightDelta = -0.10f;

    readonly List<AIStrategyRecord> _records = new();
    readonly AIGoalWeightTable _goalWeights;

    public int RecordCount => _records.Count;
    public IReadOnlyList<AIStrategyRecord> Records => _records;

    public MockStrategyLearning()
    {
        _goalWeights = AIGoalWeightTable.Shared;
    }

    public MockStrategyLearning(AIGoalWeightTable goalWeights)
    {
        _goalWeights = goalWeights ?? AIGoalWeightTable.Shared;
    }

    void IAIStrategyLearning.Record(EAIGoalType goal, in AISimulationState simulation, bool success)
    {
        AIStrategyRecord record = new AIStrategyRecord(goal, success, simulation.Score.TotalScore);
        _records.Add(record);

        float beforeWeight = _goalWeights.GetWeights(goal);

        float delta = success ? SuccessWeightDelta : FailureWeightDelta;
        _goalWeights.Adjust(goal, delta);

        ApplySituationBoost(goal, simulation);

        float afterWeight = _goalWeights.GetWeights(goal);

        AIDebugLogger.LogLearning(_records.Count, goal, success, beforeWeight, afterWeight);
        Debug.Log($"[AI Learning] Goal={goal}, Success={success}, Score={simulation.Score.TotalScore:F2}, Weight={beforeWeight:F2}->{afterWeight:F2}");
    }

    void ApplySituationBoost(EAIGoalType goal, in AISimulationState simulation)
    {
        switch (goal)
        {
            case EAIGoalType.KillNow:
                if (simulation.Score.DangerScore >= 2.5f || simulation.Score.SurvivalScore <= 3.0f)
                    _goalWeights.Adjust(goal, +0.06f);
                break;

            case EAIGoalType.TrapPlayer:
                if (simulation.Score.EscapeScore <= 1.5f)
                    _goalWeights.Adjust(goal, +0.06f);
                break;

            case EAIGoalType.ForceMistake:
                if (simulation.Score.TetrisScore >= 0.65f)
                    _goalWeights.Adjust(goal, +0.04f);
                break;

            case EAIGoalType.ApplyPressure:
                if (simulation.Score.SurvivalScore <= 8.0f && simulation.Score.EscapeScore <= 3.0f)
                    _goalWeights.Adjust(goal, +0.04f);
                break;
        }
    }
}