/// <summary>
/// AI의 목적(Goal)이 현재 상황에서 얼마나 적절한지 평가하는 인터페이스
/// </summary>
public interface IAIGoalEvaluator
{
    EAIGoalType Goal { get; }       
    float Evaluate(in AISimulationState sim);
}