/// <summary>
/// AI가 전략 판단에 사용하는 그리드 요약 정보
/// </summary>
public readonly struct GridContext
{
    public readonly int MaxHeight;              // 현재 그리드에서 가장 높은 블록 높이
    public readonly int HoleCount;              // 내부에 존재하는 빈 공간의 개수

    public GridContext(int maxHeight, int holeCount)
    {
        MaxHeight = maxHeight;
        HoleCount = holeCount;
    }
}