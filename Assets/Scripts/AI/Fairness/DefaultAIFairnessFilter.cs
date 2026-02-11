/// <summary>
/// AI 행동 실행 전 페어니스 검사
/// 압박 예산 기반 행동 제한
/// </summary>
public class DefaultAIFairnessFilter : IAIFairnessFilter
{
    readonly AIPressureBudget pressureBudget;

    public DefaultAIFairnessFilter(AIPressureBudget pressureBudget)
    {
        this.pressureBudget = pressureBudget;
    }

    bool IAIFairnessFilter.CanApply(IAIActionCandidate action)
    {
        throw new System.NotImplementedException();
    }

    bool IAIFairnessFilter.CanExecuteAction(EAIActionTagType actionTag, float pressureCost)
    {
        return pressureBudget.CanApply(pressureCost);
    }
}