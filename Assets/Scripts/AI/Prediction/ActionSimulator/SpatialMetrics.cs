/// <summary>
/// 플레이어 주변 공간 구조를 정량적으로 분석한 결과 집합
/// </summary>

public readonly struct SpatialMetrics
{
    public readonly int ReachableTileCount;   // 플레이어가 이동 가능한 전체 공간 크기 판단에 사용
    public readonly int AdjacentBlockCount;   // 주변이 얼마나 막혀있는지 판단 (압박도 계산)
    public readonly int EscapeRouteCount;     // 탈출 가능한 경로 개수 판단
    public readonly bool HasEscapeRoute;      // 탈출 경로 존재 여부 빠른 체크
    public readonly bool IsCornered;          // 플레이어가 코너에 몰렸는지 판단
    public readonly float DangerScore;        // 공간 위험도 계산 결과 (AI 의사결정에 사용)

    public SpatialMetrics(int reachableTileCount, int adjacentBlockCount, int escapeRouteCount, bool isCornered, float dangerScore)
    {
        ReachableTileCount = reachableTileCount;
        AdjacentBlockCount = adjacentBlockCount;
        EscapeRouteCount = escapeRouteCount;
        HasEscapeRoute = escapeRouteCount > 0;
        IsCornered = isCornered;
        DangerScore = dangerScore;
    }
}