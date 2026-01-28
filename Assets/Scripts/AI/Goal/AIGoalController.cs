/// <summary>
/// 목적 유지 / 전환을 관리하는 컨트롤러
/// </summary>
public static class AIGoalController
{
    public static AIGoalState UpdateGoal(in AIGoalState current, in PredictionResult prediction, in OutcomeEvaluation eval, float deltaTime)
    {
        // 강제 전환: 즉시 죽일 수 있으면 lock 무시
        if (eval.DangerScore >= 4.0f)
            return NewGoal(EAIGoalType.KillNow);

        // 기존 목적 유지
        if (current.LockTimer > 0f)
            return new AIGoalState(current.CurrentGoal, current.LockTimer - deltaTime);

        // 새 목적 선택
        EAIGoalType next = AIGoalSelector.SelectGoal(prediction, eval);
        return NewGoal(next);
    }

    static AIGoalState NewGoal(EAIGoalType goal)
    {
        return new AIGoalState(goal, AIGoalLockTime.GetLockTime(goal));
    }
}