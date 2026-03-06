/// <summary>
/// 기본 AI 목적 종료 규칙 구현체
/// </summary>
public class DefaultGoalTermination : IAIGoalTermination
{
    const float MinEscapeForPressure = 0.15f;

    public bool ShouldTerminate(EAIGoalType goal, float remainingLockTime, in AISimulationState simulation)
    {
        if (remainingLockTime <= 0f)
            return true;


        return goal switch
        {
            EAIGoalType.None => true,
            EAIGoalType.KillNow => ShouldTerminateKillNow(simulation),
            EAIGoalType.TrapPlayer => ShouldTerminateTrap(simulation),
            EAIGoalType.ForceMistake => ShouldTerminateForceMistake(simulation),
            EAIGoalType.ApplyPressure => ShouldTerminatePressure(simulation),
            _ => true
        };
    }

    static bool ShouldTerminateKillNow(in AISimulationState simulation)
    {
        bool pressureMaintained = simulation.Score.DangerScore >= 2.4f;
        bool escapeReopened = simulation.Score.EscapeScore > 0.65f || !simulation.AIThreat.IsCornered;

        return !pressureMaintained || escapeReopened;
    }

    static bool ShouldTerminateTrap(in AISimulationState simulation)
    {
        bool fullyTrapped = simulation.Score.EscapeScore <= 0.05f;
        bool trapFailed = simulation.Score.EscapeScore >= 0.75f && simulation.Score.DangerScore < 1.5f;

        return fullyTrapped || trapFailed;
    }

    static bool ShouldTerminateForceMistake(in AISimulationState simulation)
    {
        bool shouldEscalateToKill = simulation.Score.DangerScore >= 2.8f;
        bool shouldEscalateToTrap = simulation.Score.EscapeScore <= 0.2f;
        bool noLongerEffective = simulation.Score.DangerScore < 0.9f && simulation.Score.EscapeScore > 0.8f;

        return shouldEscalateToKill || shouldEscalateToTrap || noLongerEffective;
    }

    static bool ShouldTerminatePressure(in AISimulationState simulation)
    {
        bool shouldEscalate = simulation.Score.DangerScore >= 1.8f || simulation.Score.EscapeScore <= 0.3f;
        bool pressureIneffective = simulation.Score.SurvivalScore <= MinEscapeForPressure;

        return shouldEscalate || pressureIneffective;
    }
}