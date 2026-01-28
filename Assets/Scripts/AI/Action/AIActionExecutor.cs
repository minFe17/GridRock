/// <summary>
/// AI 액션 실행을 담당하며 Goal 정책 + 쿨타임 페어너스를 동시에 적용
/// </summary>
public sealed class AIActionExecutor
{
    private readonly AIActionCooldownTable _cooldowns;

    public AIActionExecutor(AIActionCooldownTable cooldowns)
    {
        _cooldowns = cooldowns;
    }

    public void TryExecute(IAIAction action, EAIGoalType goal, float currentTime, in AIActionContext context)
    {
        // 목적상 허용되지 않으면 실행 불가
        if (!AIGoalActionPolicy.IsAllowed(goal, action.ActionTag))
            return;

        // 쿨타임 페어너스
        if (!_cooldowns.CanUse(action.ActionTag, currentTime))
            return;

        action.Execute(context);
        _cooldowns.MarkUsed(action.ActionTag, currentTime);
    }
}