/// <summary>
/// AI 목적 재사용 제한
/// 동일 목적 연속 선택 방지
/// </summary>
public class AIGoalCooldown
{
    private EAIGoalType lastGoal;     // 이전 목적
    private float lastGoalTime;       // 이전 목적 시각

    public bool CanReuse(EAIGoalType goal, float currentTime, float cooldown)
    {
        if (goal != lastGoal)
            return true;

        return currentTime - lastGoalTime >= cooldown;
    }

    public void Mark(EAIGoalType goal, float currentTime)
    {
        lastGoal = goal;
        lastGoalTime = currentTime;
    }
}