/// <summary>
/// 하나의 행동(이동/점프)을 시뮬레이션 한 결과
/// 생존 여부와 탈출 가능성을 함께 반환
/// </summary>

public readonly struct SimulationResult
{
    public readonly SimulationState EndState;
    public readonly bool IsAlive;        // 깔림 / 사망 여부
    public readonly bool CanEscape;      // 현재 상태에서 탈출 가능 여부

    public SimulationResult(SimulationState endState, bool isAlive, bool canEscape)
    {
        EndState = endState;
        IsAlive = isAlive;
        CanEscape = canEscape;
    }
}