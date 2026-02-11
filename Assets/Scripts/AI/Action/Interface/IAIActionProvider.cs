using System.Collections.Generic;

/// <summary>
/// 특정 Goal 상태에서 AI가 고려할 수 있는 Action 후보 목록을 제공하는 인터페이스
/// </summary>
public interface IAIActionProvider
{
    IReadOnlyList<IAIActionCandidate> GetCandidates(EAIGoalType goal);
}