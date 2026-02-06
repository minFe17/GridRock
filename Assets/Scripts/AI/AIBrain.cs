/// <summary>
/// AI의 상위 의사결정을 담당하는 브레인 클래스
/// 예측 결과와 평가 점수를 바탕으로 현재 목적을 선택하고 일정 시간 유지
/// </summary>
public class AIBrain
{
    AIGoalState goalState;      // 현재 AI 목적과 락 정보
    readonly IAIGoalDecider goalDecider;
    readonly IAIGoalTermination termination;

    public AIBrain(IAIGoalDecider goalDecider, IAIGoalTermination termination)
    {
        this.goalDecider = goalDecider;
        this.termination = termination;
        goalState = new AIGoalState(EAIGoalType.None, 0f);
    }

    public EAIGoalType CurrentGoal => goalState.CurrentGoal;

    public void Update(float deltaTime, in AISimulationState simulation)
    {
        // 목적 락 유지
        if (goalState.LockTimer > 0f)
        {
            goalState = new AIGoalState(goalState.CurrentGoal, goalState.LockTimer - deltaTime);
            return;
        }

        // 목적 종료 조건 검사
        if (termination.ShouldTerminate(goalState.CurrentGoal, simulation))
        {
            goalState = new AIGoalState(EAIGoalType.None, 0f);
        }

        // 새 목적 선택
        EAIGoalType nextGoal = goalDecider.DecideGoal(simulation);

        float lockTime = AIGoalLockTime.GetLockTime(nextGoal);

        goalState = new AIGoalState(nextGoal, lockTime);
    }
}