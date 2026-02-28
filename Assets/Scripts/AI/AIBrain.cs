using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// AI의 상위 의사결정을 담당하는 브레인 클래스
/// 예측 결과와 평가 점수를 바탕으로 현재 목적을 선택하고 일정 시간 유지
/// </summary>
public class AIBrain
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

    public AIBrain(IAIGoalDecider goalDecider, IAIGoalTermination termination, IAIActionProvider actionProvider, AIActionSelector actionSelector, IAIStrategyLearning learning)
    {
        _goalDecider = goalDecider;
        _termination = termination;
        _actionProvider = actionProvider;
        _actionSelector = actionSelector;
        _learning = learning;

        _goalState = new AIGoalState(EAIGoalType.None, 0f);
    }

    // 현재 AI가 유지 중인 Goal
    public EAIGoalType CurrentGoal => _goalState.CurrentGoal;

    public void Update(float deltaTime, in AIInterferenceTriggerState trigger, in AIActionContext actionContext)
    {
        // 후보 액션마다 PredictedWorldState 생성
        Vector2 predictedPosition = PredictCandidatePosition(); // 후보 위치 계산
        SpatialMetrics spatial = AnalyzePlayerSpace(predictedPosition); // 공간 분석
        float futureTrapRisk = EstimateFutureRisk(predictedPosition, spatial); // 위험 예측

        PredictedWorldState predictedState = new PredictedWorldState(predictedPosition, spatial, futureTrapRisk);

        // 2. OutcomeEvaluation 생성
        OutcomeEvaluation evaluation = OutcomeEvaluator.Evaluate(predictedState);

        // 3. AISimulationState 생성
        AIContext context = SimpleSingleton<AIContextBuilder>.Instance.Build();
        BlockState blockState = BuildBlockState(context.ActiveBlock, context.Grid);
        AISimulationState simulationState = new AISimulationState(evaluation, new AIThreat(), context.Player, blockState);

        UpdateGoal(deltaTime, simulationState);
        ExecuteAction(simulationState, trigger, actionContext);
    }

    Vector2 PredictCandidatePosition()
    {
        // 임시: 현재 플레이어 위치 + 오른쪽으로 1타일 이동
        return new Vector2(0, 0); // 실제 위치로 교체 필요
    }

    SpatialMetrics AnalyzePlayerSpace(Vector2 pos)
    {
        // 임시: 임의 값 반환
        return new SpatialMetrics(reachableTileCount: 10, regionSize: 20, escapePathLength: 5, chokePointCount: 1, adjacentBlockCount: 2);
    }

    float EstimateFutureRisk(Vector2 pos, SpatialMetrics spatial)
    {
        // 임시: 안전한 상태 가정
        return 0f;
    }

    // Goal 처리
    void UpdateGoal(float deltaTime, in AISimulationState simulation)
    {
        // 1. Lock 유지 중이면 감소
        if (_goalState.LockTimer > 0f)
        {
            _goalState = new AIGoalState(_goalState.CurrentGoal, _goalState.LockTimer - deltaTime);
            return;
        }

        // 2. 종료 조건 검사
        if (_termination.ShouldTerminate(_goalState.CurrentGoal, simulation))
            _goalState = new AIGoalState(EAIGoalType.None, 0f);

        // 3. 새 Goal 결정
        EAIGoalType nextGoal = _goalDecider.DecideGoal(simulation);

        float lockTime = AIGoalLockTime.GetLockTime(nextGoal);

        _goalState = new AIGoalState(nextGoal, lockTime);
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

    BlockState BuildBlockState(BlockContext? blockContext, GridContext grid)
    {
        if (!blockContext.HasValue)
            return new BlockState(EBlockType.Max, Vector2.zero, 0, true, 0);
        float pressure = 0f;
        Vector2 pos = new Vector2(0, 0); // 실제 위치 계산 필요
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (grid.IsOccupied(pos + new Vector2(x, y)))
                    pressure += 0.5f;
            }
        }
        BlockContext block = blockContext.Value;
        return new BlockState(block.BlockType, pos, block.Rotation, true, pressure);
    }
}