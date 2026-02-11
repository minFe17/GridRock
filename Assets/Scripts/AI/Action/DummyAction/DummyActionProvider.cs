using System.Collections.Generic;

public class DummyActionProvider : IAIActionProvider
{
    public IReadOnlyList<IAIActionCandidate> GetCandidates(EAIGoalType goal)
    {
        return new IAIActionCandidate[]
        {
            new ApplyPressureActionCandidate(),
            new BlockEscapeActionCandidate(),
            new InstantKillActionCandidate()
        };
    }
}