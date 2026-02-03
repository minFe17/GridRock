/// <summary>
/// AI의 판단 결과를 기반으로 성공/실패 패턴을 기록하고 가중치를 조정하는 학습 인터페이스
/// </summary>
public interface IAIStrategyLearning
{
    void Record(EAIGoalType goal, in AISimulationState simulation, bool success);
}