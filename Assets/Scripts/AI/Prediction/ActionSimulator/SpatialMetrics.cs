/// <summary>
/// 플레이어 주변 공간 구조를 정량적으로 분석한 결과 집합
/// </summary>

public readonly struct SpatialMetrics
{
    public readonly int ReachableTileCount;   // 현재 위치에서 도달 가능한 타일 수
    public readonly int RegionSize;           // 플레이어가 속한 전체 구역 크기
    public readonly int EscapePathLength;     // 탈출 지점까지 최단 거리
    public readonly int ChokePointCount;      // 병목 지점 개수
    public readonly int AdjacentBlockCount;   // 인접한 장애물 수

    public SpatialMetrics(int reachableTileCount, int regionSize, int escapePathLength, int chokePointCount, int adjacentBlockCount)
    {
        ReachableTileCount = reachableTileCount;
        RegionSize = regionSize;
        EscapePathLength = escapePathLength;
        ChokePointCount = chokePointCount;
        AdjacentBlockCount = adjacentBlockCount;
    }
}