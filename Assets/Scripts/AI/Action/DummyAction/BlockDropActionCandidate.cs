/// <summary>
/// AI 행동 후보(ActionCandidate)
/// </summary>
sealed class BlockDropActionCandidate : IAIActionCandidate
{
    public EAIActionTagType ActionTag { get; }
    public float PressureCost { get; }
    public IAIAction Action { get; }

    public BlockDropActionCandidate(EAIActionTagType actionTag, float pressureCost, IAIAction action)
    {
        ActionTag = actionTag;
        PressureCost = pressureCost;
        Action = action;
    }
}