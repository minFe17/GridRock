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
    }

    void IAIBrain.Update(float deltaTime, in AIInterferenceTriggerState trigger, in AIActionContext actionContext)
    {
        _turnCounter++;

        AISimulationState simulation = _simulationService.Simulate(actionContext);

        UpdateGoal(deltaTime, simulation);
        ExecuteAction(simulation, trigger, actionContext);

        AIGoalWeightTable.Shared.Decay(0.02f);
        AIActionWeightTable.Shared.Decay(0.02f);
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
        IReadOnlyList<IAIActionCandidate> candidates = _actionProvider.GetCandidates(_goalState.CurrentGoal);
        if (candidates == null || candidates.Count == 0)
            return;

        IAIActionCandidate selected = _actionSelector.SelectWithReport(
            candidates, _goalState.CurrentGoal, simulation, trigger, context, out _);

        if (selected == null)
            return;

        // 실행 전 상태
        AISimulationState baseline = _simulationService.Simulate(context);

        // 실행
        selected.Action.Execute(context);

        // 실행 후 상태
        AISimulationState after = _simulationService.SimulateCandidate(context, selected);

        float baseScore = AIActionSelector.EvaluateForLearning(selected, _goalState.CurrentGoal, baseline);
        float afterScore = AIActionSelector.EvaluateForLearning(selected, _goalState.CurrentGoal, after);

        float rawDelta = afterScore - baseScore;
        float delta = ComputeLearningDelta(baseScore, afterScore);

        // 너무 작은 값 방지
        if (Mathf.Abs(delta) < 0.005f)
            delta = Random.Range(-0.01f, 0.01f);

        // 반영
        AIGoalWeightTable.Shared.Adjust(_goalState.CurrentGoal, delta);
        AIActionWeightTable.Shared.Adjust(_goalState.CurrentGoal, selected.ActionTag, delta);

        bool success = delta > 0f;
        _learning.Record(_goalState.CurrentGoal, after, success);

        Debug.Log($"[AutoTune FIX] Goal:{_goalState.CurrentGoal} Action:{selected.ActionTag} " + $"Base:{baseScore:F3} After:{afterScore:F3} Raw:{rawDelta:F3} Δ:{delta:F3} " + $"G:{AIGoalWeightTable.Shared.GetWeights(_goalState.CurrentGoal):F2} " + $"A:{AIActionWeightTable.Shared.GetWeight(_goalState.CurrentGoal, selected.ActionTag):F2}");
    }

    static float ComputeLearningDelta(float baseScore, float afterScore)
    {
        float rawDelta = afterScore - baseScore;

        // 점수 절대값으로 정규화해 과도한 편차를 줄인다.
        float scale = Mathf.Abs(baseScore) + Mathf.Abs(afterScore) + 1f;
        float normalized = rawDelta / scale;

        // tanh로 튀는 값을 압축하고 최종 범위를 ±0.2로 제한한다.
        float squashed = (float)System.Math.Tanh(normalized * 2f);
        return squashed * 0.2f;
    }
}