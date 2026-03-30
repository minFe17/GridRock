public readonly struct AIActionScoreEntry
{
    public readonly EAIActionTagType ActionTag;
    public readonly float Score;
    public readonly float PressureCost;

    public AIActionScoreEntry(EAIActionTagType actionTag, float score, float pressureCost)
    {
        ActionTag = actionTag;
        Score = score;
        PressureCost = pressureCost;
    }
}