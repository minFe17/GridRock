using System.Collections.Generic;

/// <summary>
/// AI 행동 태그 단위로 쿨타임을 관리하는 페어너스 시스템
/// 강력한 방해 행동의 연속 사용을 방지
/// </summary>
public sealed class AIActionCooldownTable
{
    private readonly Dictionary<EAIActionTag, float> cooldowns;   // 태그별 쿨타임
    private readonly Dictionary<EAIActionTag, float> lastUseTime; // 태그별 마지막 사용 시각

    public AIActionCooldownTable(Dictionary<EAIActionTag, float> cooldownConfig)
    {
        cooldowns = cooldownConfig;
        lastUseTime = new Dictionary<EAIActionTag, float>();
    }

    public bool CanUse(EAIActionTag tag, float currentTime)
    {
        if (!cooldowns.TryGetValue(tag, out float cd))
            return true;

        if (!lastUseTime.TryGetValue(tag, out float last))
            return true;

        return currentTime - last >= cd;
    }

    public void MarkUsed(EAIActionTag tag, float currentTime)
    {
        lastUseTime[tag] = currentTime;
    }
}