using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// AI 턴별 의사결정 로그를 콘솔에서 읽기 쉽게 출력
/// </summary>
public static class AIDebugLogger
{
    public static bool Enabled = true;

    public static void LogTurnHeader(int turn)
    {
        if (!Enabled)
            return;

        Debug.Log($"[AI Debug] Turn {turn}");
    }

    public static void LogGoal(int turn, EAIGoalType goal)
    {
        if (!Enabled)
            return;

        Debug.Log($"[AI Debug] Turn {turn} Goal: {goal}");
    }

    public static void LogSelection(int turn, in AIActionSelectionReport report)
    {
        if (!Enabled)
            return;

        StringBuilder builder = new StringBuilder(256);
        builder.Append("[AI Debug] Candidate Scores");
        builder.Append(" | Turn=").Append(turn);
        builder.Append(", Evaluated=").Append(report.EvaluatedCount);
        builder.Append(", Filtered=").Append(report.FilteredCount);

        if (!report.HasSelected)
        {
            builder.Append(", Selected=None");
            Debug.Log(builder.ToString());
            return;
        }

        builder.Append(", Selected=").Append(report.SelectedTag);
        builder.Append(", Score=").Append(report.SelectedScore.ToString("F2"));
        Debug.Log(builder.ToString());

        IReadOnlyList<AIActionScoreEntry> entries = report.Entries;
        int max = Mathf.Min(8, entries.Count);

        for (int i = 0; i < max; i++)
        {
            AIActionScoreEntry entry = entries[i];
            Debug.Log($"[AI Debug]   Candidate {i + 1}: Tag={entry.ActionTag}, Score={entry.Score:F2}, Pressure={entry.PressureCost:F2}");
        }
    }

    public static void LogLearning(int turn, EAIGoalType goal, bool success, float beforeWeight, float afterWeight)
    {
        if (!Enabled)
            return;

        float delta = afterWeight - beforeWeight;
        Debug.Log($"[AI Debug] Learning | Turn={turn}, Goal={goal}, Success={success}, Weight={beforeWeight:F2}->{afterWeight:F2} (Δ{delta:+0.00;-0.00;0.00})");
    }
}