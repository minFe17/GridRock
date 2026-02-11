using System.Collections.Generic;

/// <summary>
/// AI 학습 시스템의 Mock 구현체
/// </summary>
public class MockStrategyLearning : IAIStrategyLearning
{
    private readonly List<AIStrategyRecord> _records = new();

    public void Record(EAIGoalType goal, in AISimulationState simulation, bool success)
    {
        _records.Add(new AIStrategyRecord(goal, success, simulation.TotalScore));
    }

    // 디버그 / 분석용
    public IReadOnlyList<AIStrategyRecord> Records => _records;
}
