/// <summary>
/// AI가 현재 시뮬레이션된 게임 상태를 기반으로 다음에 추구할 상위 목적을 결정하는 인터페이스
/// </summary>
public interface IAIGoalDecider
{
    EAIGoalType DecideGoal(in AISimulationState simulation);
}