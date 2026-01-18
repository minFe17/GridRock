using UnityEngine;

/// <summary>
/// 격자(Grid) 기반 환경에서 하나의 행동(Action)을 가상으로 실행해보는 시뮬레이터
/// 
/// - 입력 상태(SimulationState)와 행동(EActionType)을 받아
/// - 이동 / 점프 / 중력 적용을 순차적으로 처리한 뒤
/// - 생존 여부(IsAlive)와 탈출 가능성(CanEscape)을 포함한 결과(SimulationResult)를 반환
/// 
/// </summary>
public static class ActionSimulator
{
    public static SimulationResult Simulate(in SimulationState startState, EActionType action, IGridQuery grid)
    {
        // 시작 상태 복사 (불변 구조체 유지)
        SimulationState state = startState;

        // 행동 처리
        switch (action)
        {
            case EActionType.MoveLeft:
                state = TryMove(state, Vector2Int.left, grid);
                break;

            case EActionType.MoveRight:
                state = TryMove(state, Vector2Int.right, grid);
                break;

            case EActionType.Jump:
                state = TryJump(state, grid);
                break;
        }

        // 행동 이후 항상 중력 적용
        state = ApplyGravity(state, grid);

        // 사망 여부 (위 + 현재 둘 다 막히면 깔림)
        bool isAlive = !IsCrushed(state, grid);

        // 현재 위치에서 탈출 가능성 판단
        bool canEscape = CheckEscape(state, grid);

        return new SimulationResult(state, isAlive, canEscape);
    }

    // 좌/우 이동을 시도
    static SimulationState TryMove(in SimulationState state, Vector2Int dir, IGridQuery grid)
    {
        Vector2Int next = state.GridPos + dir;

        // 이동 불가 조건
        if (!grid.IsInsideGrid(next) || grid.IsBlocked(next))
            return state;

        // 이동 성공
        return new SimulationState(next, state.IsGrounded, state.IsJumping, state.RemainingJump);
    }

    // 점프를 시도
    static SimulationState TryJump(in SimulationState state, IGridQuery grid)
    {
        // 점프 불가
        if (!state.IsGrounded || state.RemainingJump <= 0)
            return state;

        return new SimulationState(state.GridPos + Vector2Int.up, false, true, state.RemainingJump - 1);
    }

    // 중력을 적용
    static SimulationState ApplyGravity(in SimulationState state, IGridQuery grid)
    {
        Vector2Int below = state.GridPos + Vector2Int.down;

        // 아래가 비어있으면 낙하
        if (!grid.IsBlocked(below))
            return new SimulationState(below, false, state.IsJumping, state.RemainingJump);

        // 착지
        return new SimulationState(state.GridPos, true, false, state.RemainingJump);
    }

    // 현재 위치와 위쪽이 모두 막혀있으면 깔린 상태로 판단
    static bool IsCrushed(in SimulationState state, IGridQuery grid)
    {
        Vector2Int above = state.GridPos + Vector2Int.up;
        return grid.IsBlocked(state.GridPos) && grid.IsBlocked(above);
    }

    // 현재 위치에서 최소 한 방향이라도 이동 가능하면 탈출 가능하다고 판단
    static bool CheckEscape(in SimulationState state, IGridQuery grid)
    {
        return !grid.IsBlocked(state.GridPos + Vector2Int.left) || !grid.IsBlocked(state.GridPos + Vector2Int.right) || !grid.IsBlocked(state.GridPos + Vector2Int.up);
    }
}