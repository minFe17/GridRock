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

    public AIBrain(IAIGoalDecider goalDecider, IAIGoalTermination termination, IAIActionProvider actionProvider, AIActionSelector actionSelector, IAIStrategyLearning learning, IAISimulationService simulationService)
    {
        _goalDecider = goalDecider;
        _termination = termination;
        _actionProvider = actionProvider;
        _actionSelector = actionSelector;
        _learning = learning;
        _simulationService = simulationService;

        _goalState = new AIGoalState(EAIGoalType.None, 0f);
    }

    // 현재 AI가 유지 중인 Goal
    public EAIGoalType CurrentGoal => _goalState.CurrentGoal;

    void IAIBrain.Update(float deltaTime, in AIInterferenceTriggerState trigger, in AIActionContext actionContext)
    {
        AISimulationState simulation = _simulationService.Simulate(actionContext);

        UpdateGoal(deltaTime, simulation);
        ExecuteAction(simulation, trigger, actionContext);
    }

    // Goal 처리
    void UpdateGoal(float deltaTime, in AISimulationState simulation)
    {
        // 1. Lock 유지 중이면 감소
        float remainingLockTime = Mathf.Max(0f, _goalState.LockTimer - deltaTime);

        if (_goalState.CurrentGoal != EAIGoalType.None)
        {
            EAIGoalType emergencyGoal = _goalDecider.DecideGoal(simulation, _goalState.CurrentGoal, remainingLockTime, out float emergencyLockTime);

            if (emergencyGoal == EAIGoalType.KillNow && _goalState.CurrentGoal != EAIGoalType.KillNow)
            {
                _goalState = new AIGoalState(EAIGoalType.KillNow, emergencyLockTime);
                return;
            }

            if (emergencyGoal == EAIGoalType.TrapPlayer && _goalState.CurrentGoal != EAIGoalType.KillNow && _goalState.CurrentGoal != EAIGoalType.TrapPlayer)
            {
                _goalState = new AIGoalState(EAIGoalType.TrapPlayer, emergencyLockTime);
                return;
            }

            if (_goalState.CurrentGoal != EAIGoalType.None && !_termination.ShouldTerminate(_goalState.CurrentGoal, remainingLockTime, simulation))
            {
                _goalState = new AIGoalState(_goalState.CurrentGoal, remainingLockTime);
                return;
            }
        }

        EAIGoalType nextGoal = _goalDecider.DecideGoal(simulation, _goalState.CurrentGoal, remainingLockTime, out float nextLockTime);
        _goalState = new AIGoalState(nextGoal, nextLockTime);
    }

    // 현재 Goal을 기반으로 Action을 선택하고 실행한다.
    void ExecuteAction(in AISimulationState simulation, in AIInterferenceTriggerState trigger, in AIActionContext context)
    {
        // 1. 후보 수집
        IReadOnlyList<IAIActionCandidate> candidates = _actionProvider.GetCandidates(_goalState.CurrentGoal);

        if (candidates == null || candidates.Count == 0)
            return;

        // 2. Action 선택
        IAIActionCandidate selected = _actionSelector.Select(candidates, _goalState.CurrentGoal, simulation, trigger, context);

        if (selected == null)
            return;

        // 3. 실행
        selected.Action.Execute(context);

        // 4. 결과 평가 (단순 예시)
        AISimulationState simulationAfter = _simulationService.SimulateCandidate(context, selected);
        bool success = EvaluateResult(simulation, simulationAfter, _goalState.CurrentGoal);

        // 5. Learning 기록
        _learning.Record(_goalState.CurrentGoal, simulationAfter, success);
    }

    // Action 실행 결과 평가.
    static bool EvaluateResult(in AISimulationState before, in AISimulationState after, EAIGoalType goal)
    {
        float reachableDelta = before.Score.SurvivalScore - after.Score.SurvivalScore;
        float dangerDelta = after.Score.DangerScore - before.Score.DangerScore;
        float escapeDelta = before.Score.EscapeScore - after.Score.EscapeScore;

        bool reducedReachableArea = reachableDelta > 0.25f;
        bool increasedDanger = dangerDelta > 0.10f;
        bool reducedEscapeRoutes = escapeDelta > 0.05f;

        return goal switch
        {
            EAIGoalType.KillNow => (increasedDanger && (reducedReachableArea || reducedEscapeRoutes)) || after.Score.SurvivalScore <= 0f,
            EAIGoalType.TrapPlayer => reducedEscapeRoutes || after.Score.EscapeScore <= 0.05f,
            EAIGoalType.ForceMistake => increasedDanger || (reducedReachableArea && reducedEscapeRoutes),
            EAIGoalType.ApplyPressure => reducedReachableArea || increasedDanger || reducedEscapeRoutes,
            _ => reducedReachableArea || increasedDanger || reducedEscapeRoutes,
        };
    }
}