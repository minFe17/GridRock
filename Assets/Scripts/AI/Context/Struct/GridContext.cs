using UnityEngine;

/// <summary>
/// AI가 전략 판단에 사용하는 그리드 요약 정보
/// </summary>
public readonly struct GridContext
{
    public readonly bool[,] Occupancy;          // true = 블록 있음
    public readonly int HoleCount;

    public readonly int MaxHeight
    {
        get
        {
            int rows = Occupancy.GetLength(0);
            int cols = Occupancy.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (Occupancy[y, x])
                        return y; // 가장 위에 있는 블록 y 인덱스 반환
                }
            }
            return rows; // 블록이 없는 경우
        }
    }

    public GridContext(bool[,] occupancy, int holeCount = 0)
    {
        Occupancy = occupancy;
        HoleCount = holeCount;
    }

    public bool IsOccupied(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x < 0 || x >= Occupancy.GetLength(0) || y < 0 || y >= Occupancy.GetLength(1))
            return false;
        return Occupancy[x, y];
    }
}