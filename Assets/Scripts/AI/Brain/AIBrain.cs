using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI의 목표/행동 의사결정을 수행하는 브레인
/// </summary>
public class AIBrain : IAIBrain
{
    // Goal 관련
    AIGoalState _goalState;
    readonly IAIGoalDecider _goalDecider;
    readonly IAIGoalTermination _termination;

    // Action 관련
    readonly IAIActionProvider _actionProvider;
    readonly AIActionSelector _actionSelector;

    // Learning
    readonly IAIStrategyLearning _learning;

    readonly IAISimulationService _simulationService;

    int _turnCounter;
    EAIActionTagType _lastActionTag;
    int _sameActionStreak;
    float _deltaBiasEma;

    // 현재 AI가 유지 중인 Goal
    public EAIGoalType CurrentGoal => _goalState.CurrentGoal;

    public AIBrain(IAIGoalDecider goalDecider, IAIGoalTermination termination, IAIActionProvider actionProvider, AIActionSelector actionSelector, IAIStrategyLearning learning, IAISimulationService simulationService)
    {
        _goalDecider = goalDecider;
        _termination = termination;
        _actionProvider = actionProvider;
        _actionSelector = actionSelector;
        _learning = learning;
        _simulationService = simulationService;

        _goalState = new AIGoalState(EAIGoalType.None, 0f);
        _turnCounter = 0;
        _lastActionTag = EAIActionTagType.ApplyPressure;
        _sameActionStreak = 0;
        _deltaBiasEma = 0f;
    }

    void IAIBrain.Update(float deltaTime, in AIInterferenceTriggerState trigger, in AIActionContext actionContext)
    {
        _turnCounter++;

        AISimulationState simulation = _simulationService.Simulate(actionContext);

        UpdateGoal(deltaTime, simulation);
        ExecuteAction(simulation, trigger, actionContext);

        AIGoalWeightTable.Shared.Decay(0.02f);
        AIActionWeightTable.Shared.Decay(0.02f);

        AIGoalWeightTable.Shared.Normalize();
        AIActionWeightTable.Shared.NormalizeByGoal();
    }

    // Goal 처리
    void UpdateGoal(float deltaTime, in AISimulationState simulation)
    {
        // Lock 유지 중이면 감소
        float remainingLockTime = Mathf.Max(0f, _goalState.LockTimer - deltaTime);

        if (_goalState.CurrentGoal != EAIGoalType.None)
        {
            if (!_termination.ShouldTerminate(_goalState.CurrentGoal, remainingLockTime, simulation))
            {
                _goalState = new AIGoalState(_goalState.CurrentGoal, remainingLockTime);
                return;
            }
        }

        EAIGoalType nextGoal = _goalDecider.DecideGoal(simulation, _goalState.CurrentGoal, remainingLockTime, out float nextLockTime);

        _goalState = new AIGoalState(nextGoal, nextLockTime);

        // CSV 로그
        AILogCSVLogger.LogGoal(_turnCounter, nextGoal);
    }

    // Action 실행
    void ExecuteAction(in AISimulationState simulation, in AIInterferenceTriggerState trigger, in AIActionContext context)
    {
        var candidates = _actionProvider.GetCandidates(_goalState.CurrentGoal);
        if (candidates == null || candidates.Count == 0)
            return;

        var selected = _actionSelector.SelectWithReport(
            candidates, _goalState.CurrentGoal, simulation, trigger, context, out _);

        if (selected == null)
            return;

        AISimulationState baseline = simulation;

        selected.Action.Execute(context);

        AISimulationState after = _simulationService.SimulateCandidate(context, selected);

        float baseScore = AIActionSelector.EvaluateForLearning(selected, _goalState.CurrentGoal, baseline);
        float afterScore = AIActionSelector.EvaluateForLearning(selected, _goalState.CurrentGoal, after);

        float referenceScore = ComputeAlternativeReference(candidates, selected, _goalState.CurrentGoal, context, baseScore);

        float delta = ComputeLearningDelta(baseScore, afterScore, referenceScore);

        float diff = afterScore - baseScore;

        if (Mathf.Abs(diff) < 0.01f)
            delta -= 0.01f * Random.Range(0.5f, 1.5f);

        UpdateActionStreak(selected.ActionTag);

        if (_sameActionStreak >= 3)
            delta -= 0.015f * (_sameActionStreak - 2);

        if (_goalState.CurrentGoal == EAIGoalType.ApplyPressure && _sameActionStreak >= 4)
            delta -= 0.01f * (_sameActionStreak - 3);

        if (Mathf.Abs(delta) < 0.002f)
            delta = 0f;

        delta = Mathf.Clamp(delta, -0.2f, 0.2f);

        AIGoalWeightTable.Shared.Adjust(_goalState.CurrentGoal, delta);
        AIActionWeightTable.Shared.Adjust(_goalState.CurrentGoal, selected.ActionTag, delta);

        bool success = afterScore > Mathf.Max(baseScore, referenceScore);
        _learning.Record(_goalState.CurrentGoal, after, success);

        Debug.Log($"[AutoTune FIX] Goal:{_goalState.CurrentGoal} Action:{selected.ActionTag} " + $"Base:{baseScore:F3} After:{afterScore:F3} Ref:{referenceScore:F3} Δ:{delta:F3} Streak:{_sameActionStreak} " + $"G:{AIGoalWeightTable.Shared.GetWeights(_goalState.CurrentGoal):F2} " + $"A:{AIActionWeightTable.Shared.GetWeight(_goalState.CurrentGoal, selected.ActionTag):F2}");
    }

    float ComputeAlternativeReference(IReadOnlyList<IAIActionCandidate> candidates, IAIActionCandidate selected, EAIGoalType goal, in AIActionContext context, float fallbackScore)
    {
        float sum = 0f;
        int count = 0;

        foreach (IAIActionCandidate candidate in candidates)
        {
            if (candidate == null || candidate == selected)
                continue;

            AISimulationState altState = _simulationService.SimulateCandidate(context, candidate);
            if (altState.Equals(default))
                continue;

            sum += AIActionSelector.EvaluateForLearning(candidate, goal, altState);
            count++;
        }

        return count > 0 ? sum / count : fallbackScore;
    }

    void UpdateActionStreak(EAIActionTagType actionTag)
    {
        if (actionTag == _lastActionTag)
            _sameActionStreak++;
        else
        {
            _lastActionTag = actionTag;
            _sameActionStreak = 1;
        }
    }

    static float ComputeLearningDelta(float baseScore, float afterScore, float referenceScore)
    {
        float baseline = Mathf.Lerp(baseScore, referenceScore, 0.5f);
        float raw = afterScore - baseline;
        float normalized = raw / (Mathf.Abs(baseline) + 1f);

        float squashed = (float)System.Math.Tanh(normalized * 2.0f);
        float delta = squashed * 0.15f;

        if (Mathf.Abs(delta) < 0.005f)
            delta = Mathf.Sign(raw) * 0.01f;

        return delta;
    }
}