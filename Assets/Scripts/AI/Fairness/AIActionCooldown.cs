using System.Collections.Generic;

/// <summary>
/// AI 액션 연속 실행 제한
/// 행동 스팸 방지
/// </summary>
public class AIActionCooldown
{
    private readonly Dictionary<EAIActionTagType, float> lastExecuteTime; // 마지막 실행 시각
    private readonly Dictionary<EAIActionTagType, float> cooldowns;       // 액션별 쿨타임

    public AIActionCooldown(Dictionary<EAIActionTagType, float> cooldowns)
    {
        this.cooldowns = cooldowns;
        lastExecuteTime = new Dictionary<EAIActionTagType, float>();
    }

    public bool CanExecute(EAIActionTagType tag, float currentTime)
    {
        if (!lastExecuteTime.TryGetValue(tag, out float lastTime))
            return true;

        return currentTime - lastTime >= cooldowns[tag];
    }

    public void MarkExecuted(EAIActionTagType tag, float currentTime)
    {
        lastExecuteTime[tag] = currentTime;
    }
}
