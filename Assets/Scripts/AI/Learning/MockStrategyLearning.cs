using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 전략 결과를 기록하고, 성공률 기반 점수 보정값을 제공하는 Mock 학습 시스템
/// </summary>
public class MockStrategyLearning : IAIStrategyLearning
{
    readonly List<AIStrategyRecord> _records = new();

    public int RecordCount => _records.Count;
    public IReadOnlyList<AIStrategyRecord> Records => _records;

    void IAIStrategyLearning.Record(EAIGoalType goal, in AISimulationState simulation, bool success)
    {
        AIStrategyRecord record = new AIStrategyRecord(goal, success, simulation.TotalScore);

        _records.Add(record);

        Debug.Log($"[AI Learning] Goal={goal}, Success={success}, Score={simulation.TotalScore}");
    }
}