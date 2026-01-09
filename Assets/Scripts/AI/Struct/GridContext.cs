public readonly struct GridContext
{
    public readonly int Width;
    public readonly int Height;

    public readonly bool[,] Occupied;    // 고정 블록 여부
    public readonly int[,] HeightMap;    // 각 열의 최고 높이

    public readonly int ExitCount;       // 현재 플레이어 탈출 경로 수
    public readonly float EnclosureRate; // 봉쇄도 (0~1)

    public GridContext(int width, int height, bool[,] occupied, int[,] heightMap, int exitCount, float enclosureRate)
    {
        Width = width;
        Height = height;
        Occupied = occupied;
        HeightMap = heightMap;
        ExitCount = exitCount;
        EnclosureRate = enclosureRate;
    }
}