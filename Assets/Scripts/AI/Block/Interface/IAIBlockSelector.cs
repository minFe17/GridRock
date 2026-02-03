using System.Collections.Generic;

/// <summary>
/// 제공된 블록 후보 중에서AI 목적과 시뮬레이션 상태를 고려하여 실제 사용할 블록을 선택하는 인터페이스
/// </summary>
public interface IAIBlockSelector
{
    IReadOnlyList<IAIActionCandidate> Select(IReadOnlyList<IAIActionCandidate> candidates, in AISimulationState simulation, EAIGoalType goal);
}