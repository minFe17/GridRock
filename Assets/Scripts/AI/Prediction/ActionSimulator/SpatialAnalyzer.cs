using UnityEngine;

public static class SpatialAnalyzer
{
    public static SpatialMetrics Analyze(bool[,] board, Vector2 playerPos)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        Vector2Int start = new Vector2Int((int)playerPos.x, (int)playerPos.y);

        if (!IsInBounds(start, width, height))
            return new SpatialMetrics(0, 0);

        if (board[start.x, start.y])
            return new SpatialMetrics(0, CountAdjacentBlocks(board, start, width, height));

        int leftRun = CountRun(board, start, width, height, Vector2Int.left);
        int rightRun = CountRun(board, start, width, height, Vector2Int.right);

        // 플레이어 중심 좌우 생존 공간: 현재 칸 + 좌/우 연속 이동 가능 칸
        int reachableTiles = 1 + leftRun + rightRun;
        int adjacentBlocks = CountAdjacentBlocks(board, start, width, height);

        return new SpatialMetrics(reachableTiles, adjacentBlocks);
    }

    private static int CountRun(bool[,] board, Vector2Int start, int width, int height, Vector2Int direction)
    {
        int run = 0;
        Vector2Int cursor = start + direction;

        while (IsInBounds(cursor, width, height) && !board[cursor.x, cursor.y])
        {
            run++;
            cursor += direction;
        }

        return run;
    }

    private static int CountAdjacentBlocks(bool[,] board, Vector2Int pos, int width, int height)
    {
        int count = 0;

        Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int next = pos + dir;

            if (!IsInBounds(next, width, height))
                continue;

            if (board[next.x, next.y])
                count++;
        }

        return count;
    }

    private static bool IsInBounds(Vector2Int pos, int width, int height)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
}
