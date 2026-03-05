/// <summary>
/// 플레이어 주변 공간 구조를 정량적으로 분석한 결과 집합
/// </summary>

public readonly struct SpatialMetrics
{
    public readonly int ReachableTileCount;   // 현재 위치에서 도달 가능한 타일 수
    public readonly int AdjacentBlockCount;   // 인접한 장애물 수
    public readonly int EscapeRouteCount;
    public readonly bool HasEscapeRoute;
    public readonly bool IsCornered;
    public readonly float DangerScore;

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