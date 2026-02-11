/// <summary>
/// AI 행동 실행 전 페어니스 검사
/// </summary>
public static class AIFairnessFilter
{
    public static bool CanExecuteAction(EAIActionTagType actionTag, AIActionCooldown actionCooldown, AIPressureBudget pressureBudget, float pressureCost, float currentTime)
    {
        if (!actionCooldown.CanExecute(actionTag, currentTime))
            return false;

        if (!pressureBudget.CanApply(pressureCost))
            return false;

        return true;
    }
}