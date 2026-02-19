using System.Collections.Generic;

public class AIActionSelector
{
    readonly IAIFairnessFilter _fairnessFilter;

    public AIActionSelector(IAIFairnessFilter fairnessFilter)
    {
        _fairnessFilter = fairnessFilter;
    }

    public IAIActionCandidate Select(IReadOnlyList<IAIActionCandidate> candidates, EAIGoalType goal, in AISimulationState simulationState, in AIInterferenceTriggerState trigger)
    {
        bool allowInterfere = AIInterferencePolicy.CanInterfere(goal, trigger);

        IAIActionCandidate best = null;
        float bestScore = float.MinValue;

        foreach (IAIActionCandidate candidate in candidates)
        {
            if (!AIGoalActionPolicy.IsAllowed(goal, candidate.ActionTag))
                continue;

            if (!allowInterfere && candidate.ActionTag == EAIActionTagType.ApplyPressure)
                continue;

            if (!_fairnessFilter.CanApply(candidate))
                continue;

            if (!candidate.Action.CanExecute(simulationState))
                continue;

            float score = Evaluate(candidate, goal, trigger);

            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private float Evaluate(IAIActionCandidate candidate, EAIGoalType goal, in AIInterferenceTriggerState trigger)
    {
        float score = 0f;

        // 기본 전략 가중치
        score -= candidate.PressureCost;

        return score;
    }
}