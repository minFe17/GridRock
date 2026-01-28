public static class AIGoalLockTime
{
    public static float GetLockTime(EAIGoalType goal)
    {
        return goal switch
        {
            EAIGoalType.KillNow => 0.1f,
            EAIGoalType.TrapPlayer => 2.0f,
            EAIGoalType.ForceMistake => 1.2f,
            EAIGoalType.ApplyPressure => 3.0f,
            _ => 0f,
        };
    }
}