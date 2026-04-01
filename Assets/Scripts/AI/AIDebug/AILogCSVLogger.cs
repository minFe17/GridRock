using System.IO;
using UnityEngine;

public static class AILogCSVLogger
{
    static string path = Path.Combine(Application.dataPath, "AI_GoalLog.csv");
    static bool headerWritten = false;

    public static void LogGoal(int turn, EAIGoalType goal)
    {
        if (!headerWritten)
        {
            File.WriteAllText(path, "Turn,Goal\n");
            headerWritten = true;
        }

        string line = $"{turn},{goal}\n";
        File.AppendAllText(path, line);
    }
}