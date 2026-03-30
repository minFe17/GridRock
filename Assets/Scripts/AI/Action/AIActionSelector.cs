using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 행동 후보를 평가하고 최적의 액션을 선택하는 클래스
/// </summary>
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

    // 후보 시뮬레이션 실행 (없으면 default 반환)
    AISimulationState SimulateCandidate(IAIActionCandidate candidate, in AIActionContext actionContext)
    {
        if (_simulationService == null)
            return default;

        return _simulationService.SimulateCandidate(actionContext, candidate);
    }

    // 후보 행동 점수 평가
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
            default:
                goalScore = eval.TotalScore;
                break;
        }

        float actionBonus = 0f;

        // 행동 타입 보너스
        if (candidate.ActionTag == EAIActionTagType.InstantKill && goal == EAIGoalType.KillNow)
            actionBonus = 5f;
        else if (candidate.ActionTag == EAIActionTagType.BlockEscape && goal == EAIGoalType.TrapPlayer)
            actionBonus = 3f;
        else if (candidate.ActionTag == EAIActionTagType.CreateDanger && goal == EAIGoalType.ForceMistake)
            actionBonus = 3f;
        else if (candidate.ActionTag == EAIActionTagType.ApplyPressure && goal == EAIGoalType.ApplyPressure)
            actionBonus = 2f;

        float triggerBonus = 0f;

        // 트리거 기반 보너스
        if (allowInterfere)
        {
            if (trigger.IsNearTetris && candidate.ActionTag == EAIActionTagType.ApplyPressure)
                triggerBonus += 1.5f;

            if (trigger.IsNearLineClear &&
                (candidate.ActionTag == EAIActionTagType.BlockEscape ||
                 candidate.ActionTag == EAIActionTagType.CreateDanger))
                triggerBonus += 1.2f;

            if (trigger.IsHighStack)
                triggerBonus += 0.8f;
        }

        float placementBonus = 0f;

        // 블록 배치 기반 보너스 계산
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

            // 미래 위치 기반 보정
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

                float multiplier = goal == EAIGoalType.KillNow ? 1.8f : 1f;
                placementBonus += bestFutureCloseness * multiplier;
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

    // 후보 리스트 중 최적 행동 선택
    public IAIActionCandidate Select(IReadOnlyList<IAIActionCandidate> candidates, EAIGoalType goal, in AISimulationState simulationState, in AIInterferenceTriggerState trigger, in AIActionContext actionContext)
    {
        return SelectWithReport(candidates, goal, simulationState, trigger, actionContext, out _);
    }

    public IAIActionCandidate SelectWithReport(IReadOnlyList<IAIActionCandidate> candidates, EAIGoalType goal, in AISimulationState simulationState, in AIInterferenceTriggerState trigger, in AIActionContext actionContext, out AIActionSelectionReport report)
    { 
        bool allowInterfere = AIInterferencePolicy.CanInterfere(goal, trigger);

        IAIActionCandidate best = null;
        float bestScore = float.MinValue;

        int filteredCount = 0;
        int evaluatedCount = 0;
        List<AIActionScoreEntry> entries = new List<AIActionScoreEntry>(candidates.Count);

        foreach (IAIActionCandidate candidate in candidates)
        {
            // 목표에 맞는 행동인지 체크
            if (!AIGoalActionPolicy.IsAllowed(goal, candidate.ActionTag))
            {
                filteredCount++;
                continue;
            }

            // 간섭 허용 여부 체크
            if (!allowInterfere && candidate.ActionTag == EAIActionTagType.ApplyPressure)
            {
                filteredCount++;
                continue;
            }

            // 공정성 필터 체크
            if (!_fairnessFilter.CanApply(candidate))
            {
                filteredCount++;
                continue;
            }

            // 후보 시뮬레이션 실행
            AISimulationState candidateSimulation = SimulateCandidate(candidate, actionContext);

            // 시뮬레이션 실패 필터
            if (candidateSimulation.Equals(default))
            {
                filteredCount++;
                continue;
            }

            // 실행 가능 여부 체크
            if (!candidate.Action.CanExecute(candidateSimulation))
            {
                filteredCount++;
                continue;
            }

            // 점수 계산
            float score = Evaluate(candidate, goal, candidateSimulation, trigger, allowInterfere);
            evaluatedCount++;
            entries.Add(new AIActionScoreEntry(candidate.ActionTag, score, candidate.PressureCost));

            // 최고 점수 갱신
            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        entries.Sort((a, b) => b.Score.CompareTo(a.Score));

        if (best == null)
        {
            report = new AIActionSelectionReport(entries, evaluatedCount, filteredCount, false, EAIActionTagType.ApplyPressure, float.MinValue);
            return null;
        }

        report = new AIActionSelectionReport(entries, evaluatedCount, filteredCount, true, best.ActionTag, bestScore);

        return best;
    }
}