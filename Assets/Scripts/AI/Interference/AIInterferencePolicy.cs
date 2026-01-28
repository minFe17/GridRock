/// <summary>
/// 현재 목표와 트리거 상태를 기반으로 방해 행동을 허용할지 판단하는 정책
/// </summary>
public static class AIInterferencePolicy
{
    public static bool CanInterfere(EAIGoalType goal, in AIInterferenceTriggerState trigger)
    {
        // 성공 직전이 아니면 방해하지 않음
        if (!trigger.IsCommitMoment)
            return false;

        switch (goal)
        {
            case EAIGoalType.KillNow:
                return trigger.IsNearTetris || trigger.IsHighStack;

            case EAIGoalType.TrapPlayer:
                return trigger.IsNearLineClear;

            case EAIGoalType.ForceMistake:
                return trigger.IsNearLineClear || trigger.IsNearTetris;

            case EAIGoalType.ApplyPressure:
                return trigger.IsHighStack;

            default:
                return false;
        }
    }
}