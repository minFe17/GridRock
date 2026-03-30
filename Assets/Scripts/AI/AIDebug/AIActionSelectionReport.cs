using System;
using System.Collections.Generic;

public readonly struct AIActionSelectionReport
{
    public static readonly AIActionSelectionReport Empty = new AIActionSelectionReport(Array.Empty<AIActionScoreEntry>(), 0, 0, false, EAIActionTagType.ApplyPressure, float.MinValue);

    public readonly IReadOnlyList<AIActionScoreEntry> Entries;
    public readonly int EvaluatedCount;
    public readonly int FilteredCount;
    public readonly bool HasSelected;
    public readonly EAIActionTagType SelectedTag;
    public readonly float SelectedScore;

    public AIActionSelectionReport(IReadOnlyList<AIActionScoreEntry> entries, int evaluatedCount, int filteredCount, bool hasSelected, EAIActionTagType selectedTag, float selectedScore)
    {
        Entries = entries;
        EvaluatedCount = evaluatedCount;
        FilteredCount = filteredCount;
        HasSelected = hasSelected;
        SelectedTag = selectedTag;
        SelectedScore = selectedScore;
    }
}