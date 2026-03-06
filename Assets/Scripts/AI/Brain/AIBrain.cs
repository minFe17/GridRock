using System.Collections.Generic;
using UnityEngine;
using Utils;

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
        if (_goalState.LockTimer > 0f)
        {
            EAIGoalType emergencyGoal = _goalDecider.DecideGoal(simulation, _goalState.CurrentGoal, _goalState.LockTimer, out float emergencyLockTime);

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

            _goalState = new AIGoalState(_goalState.CurrentGoal, _goalState.LockTimer - deltaTime);
            return;
        }

        // 2. 종료 조건 검사
        if (_termination.ShouldTerminate(_goalState.CurrentGoal, simulation))
            _goalState = new AIGoalState(EAIGoalType.None, 0f);

        // 3. 새 Goal 결정
        EAIGoalType nextGoal = _goalDecider.DecideGoal(simulation, _goalState.CurrentGoal, _goalState.LockTimer, out float nextLockTime);

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
        IAIActionCandidate selected = _actionSelector.Select(candidates, _goalState.CurrentGoal, simulation, trigger);

        if (selected == null)
            return;

        // 3. 실행
        selected.Action.Execute(context);

        // 4. 결과 평가 (단순 예시)
        bool success = EvaluateResult(simulation);

        // 5. Learning 기록
        _learning.Record(_goalState.CurrentGoal, simulation, success);
    }

    // Action 실행 결과 평가.
    bool EvaluateResult(in AISimulationState simulation)
    {
        // 실제 성공 기준 정의 필요
        // 예: 상대 스택 증가, 위기 상태 유도 등

        return true;
    }
}