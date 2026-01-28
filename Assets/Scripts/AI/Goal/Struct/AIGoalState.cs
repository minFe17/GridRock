/// <summary>
/// 현재 AI 목적과 유지 상태
/// 목적은 일정 시간 유지되며 조건에 따라 강제 전환
/// </summary>
public readonly struct AIGoalState
{
    public EAIGoalType CurrentGoal { get; }
    public float LockTimer { get; }     // 목적 유지 시간

    public AIGoalState(EAIGoalType goal, float lockTimer)
    {
        CurrentGoal = goal;
        LockTimer = lockTimer;
    }

    public bool IsLocked => LockTimer > 0f;
}