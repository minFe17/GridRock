/// <summary>
/// 현재 AI 목적에서 다른 목적으로 전환이 가능한지 판단하는 정책 인터페이스
/// </summary>
public interface IAIGoalTransitionPolicy
{
    bool CanTransition(EAIGoalType from, EAIGoalType to);
}