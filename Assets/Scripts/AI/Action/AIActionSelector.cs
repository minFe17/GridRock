using System.Collections.Generic;
using UnityEngine;

public class AIActionSelector
{
    readonly IAIFairnessFilter _fairnessFilter;
    readonly IAISimulationService _simulationService;

    public AIActionSelector(IAIFairnessFilter fairnessFilter)
    {
        _fairnessFilter = fairnessFilter;
    }

    public AIActionSelector(IAIFairnessFilter fairnessFilter, IAISimulationService simulationService)
    {
        _fairnessFilter = fairnessFilter;
        _simulationService = simulationService;
    }

    public IAIActionCandidate Select(IReadOnlyList<IAIActionCandidate> candidates, EAIGoalType goal, in AISimulationState simulationState, in AIInterferenceTriggerState trigger, in AIActionContext actionContext)
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

            AISimulationState candidateSimulation = SimulateCandidateOrFallback(candidate, actionContext, simulationState);

            if (!candidate.Action.CanExecute(candidateSimulation))
                continue;

            float score = Evaluate(candidate, goal, candidateSimulation, trigger, allowInterfere);

            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    AISimulationState SimulateCandidateOrFallback(IAIActionCandidate candidate, in AIActionContext actionContext, in AISimulationState fallback)
    {
        if (_simulationService == null)
            return fallback;

        return _simulationService.SimulateCandidate(actionContext, candidate);
    }

    static float Evaluate(IAIActionCandidate candidate, EAIGoalType goal, in AISimulationState simulationState, in AIInterferenceTriggerState trigger, bool allowInterfere)
    {
        OutcomeEvaluation eval = simulationState.Score;

        float goalScore;
        switch (goal)
        {
            case EAIGoalType.KillNow:
                goalScore = eval.DangerScore * 4f - eval.EscapeScore * 3f;
                break;
            case EAIGoalType.TrapPlayer:
                goalScore = -eval.EscapeScore * 4f + eval.DangerScore * 2f;
                break;
            case EAIGoalType.ForceMistake:
                goalScore = eval.DangerScore * 2.5f - eval.SurvivalScore;
                break;
            case EAIGoalType.ApplyPressure:
            default:
                goalScore = eval.TotalScore;
                break;
        }

        float actionBonus = 0f;
        if (candidate.ActionTag == EAIActionTagType.InstantKill && goal == EAIGoalType.KillNow)
            actionBonus = 5f;
        else if (candidate.ActionTag == EAIActionTagType.BlockEscape && goal == EAIGoalType.TrapPlayer)
            actionBonus = 3f;
        else if (candidate.ActionTag == EAIActionTagType.CreateDanger && goal == EAIGoalType.ForceMistake)
            actionBonus = 3f;
        else if (candidate.ActionTag == EAIActionTagType.ApplyPressure && goal == EAIGoalType.ApplyPressure)
            actionBonus = 2f;

        float triggerBonus = 0f;
        if (allowInterfere)
        {
            if (trigger.IsNearTetris && candidate.ActionTag == EAIActionTagType.ApplyPressure)
                triggerBonus += 1.5f;

            if (trigger.IsNearLineClear && (candidate.ActionTag == EAIActionTagType.BlockEscape || candidate.ActionTag == EAIActionTagType.CreateDanger))
                triggerBonus += 1.2f;

            if (trigger.IsHighStack)
                triggerBonus += 0.8f;
        }

        float placementBonus = 0f;
        if (candidate.Action is BlockDropAction dropAction)
        {
            Vector2Int playerCell = simulationState.PlayerInfo.GridPosition;
            float distance = Mathf.Abs(dropAction.DropCell.x - playerCell.x);
            float closeness = 1f / (1f + distance);

            placementBonus += closeness * 2f;

            if (goal == EAIGoalType.KillNow)
                placementBonus += closeness * 3f;
            else if (goal == EAIGoalType.TrapPlayer)
                placementBonus += closeness * 2f;

            if (dropAction.PredictedXs != null && dropAction.PredictedXs.Count > 0)
            {
                float bestFutureCloseness = 0f;

                for (int i = 0; i < dropAction.PredictedXs.Count; i++)
                {
                    float futureDistance = Mathf.Abs(dropAction.DropCell.x - dropAction.PredictedXs[i]);
                    float futureCloseness = 1f / (1f + futureDistance);
                    if (futureCloseness > bestFutureCloseness)
                        bestFutureCloseness = futureCloseness;
                }

                float futureAimMultiplier = goal == EAIGoalType.KillNow ? 1.8f : 1f;
                placementBonus += bestFutureCloseness * futureAimMultiplier;
            }
            else if (simulationState.PlayerInfo.MoveDirection != 0)
            {
                int predictedX = playerCell.x + simulationState.PlayerInfo.MoveDirection;
                float futureDistance = Mathf.Abs(dropAction.DropCell.x - predictedX);
                placementBonus += 1f / (1f + futureDistance);
            }
        }
        return goalScore + actionBonus + triggerBonus + placementBonus - candidate.PressureCost;
    }
}