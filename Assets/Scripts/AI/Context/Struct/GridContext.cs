using UnityEngine;

/// <summary>
/// AI가 전략 판단에 사용하는 그리드 요약 정보
/// </summary>
public readonly struct GridContext
{
    public readonly bool[,] Occupancy;          
    public readonly int HoleCount;

    public readonly int MaxHeight
    {
        get
        {
            int width = Occupancy.GetLength(0);
            int height = Occupancy.GetLength(1);

            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    if (Occupancy[x, y])
                        return y+1;
                }
            }

            return 0;
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