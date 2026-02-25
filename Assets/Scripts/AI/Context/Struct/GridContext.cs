/// <summary>
/// AI가 전략 판단에 사용하는 그리드 요약 정보
/// </summary>
public readonly struct GridContext
{
    public readonly int MaxHeight;              // 현재 그리드에서 가장 높은 블록 높이
    public readonly int HoleCount;              // 내부에 존재하는 빈 공간의 개수
    public readonly int ExitCount;              // 플레이어가 탈출 가능한 경로 수
    public readonly float EnclosureRate;        // 플레이어가 봉쇄된 정도
    public readonly bool JumpRouteAvailable;    // 위쪽으로 탈출 가능한 경로가 존재하는지 여부

    public GridContext(int maxHeight, int holeCount, int exitCount, float enclosureRate, bool jumpRouteAvailable)
    {
        MaxHeight = maxHeight;
        HoleCount = holeCount;
        ExitCount = exitCount;
        EnclosureRate = enclosureRate;
        JumpRouteAvailable = jumpRouteAvailable;
    }
}