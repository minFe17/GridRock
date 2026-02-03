using System.Collections.Generic;

/// <summary>
/// 선택된 블록들을 어떤 순서로 사용할지 결정하는 인터페이스
/// </summary>
public interface IAIBlockOrderPlanner
{
    IReadOnlyList<IAIActionCandidate> DecideOrder(IReadOnlyList<IAIActionCandidate> blocks, EAIGoalType goal);
}