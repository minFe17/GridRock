using UnityEngine;

public static class SpatialAnalyzer
{
    static readonly Vector2Int[] Directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    public static SpatialMetrics Analyze(bool[,] board, Vector2 playerPos)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        Vector2Int start = new Vector2Int((int)playerPos.x, (int)playerPos.y);

        if (!IsInBounds(start, width, height))
            return new SpatialMetrics(0, 0, 0, true, 10f);

        if (board[start.x, start.y])
            return new SpatialMetrics(0, CountAdjacentBlocks(board, start, width, height), 0, true, 10f);

        int leftRun = CountRun(board, start, width, height, Vector2Int.left);
        int rightRun = CountRun(board, start, width, height, Vector2Int.right);

        // 플레이어 중심 좌우 생존 공간: 현재 칸 + 좌/우 연속 이동 가능 칸
        int reachableTiles = 1 + leftRun + rightRun;
        int adjacentBlocks = CountAdjacentBlocks(board, start, width, height);

        // 탈출 루트 존재 판단
        int escapeRouteCount = CountEscapeRoutes(start, width, leftRun, rightRun);

        // 코너 몰림 판단
        bool isCornered = IsCornered(board, start, width, height, escapeRouteCount, reachableTiles);

        // 공간 기반 DangerScore 계산
        float dangerScore = CalculateDangerScore(width, reachableTiles, adjacentBlocks, escapeRouteCount, isCornered);

        return new SpatialMetrics(reachableTiles, adjacentBlocks, escapeRouteCount, isCornered, dangerScore);
    }

    static int CountEscapeRoutes(Vector2Int start, int width, int leftRun, int rightRun)
    {
        int routes = 0;

        bool canReachLeftBoundary = start.x - leftRun <= 0;
        bool canReachRightBoundary = start.x + rightRun >= width - 1;

        if (canReachLeftBoundary)
            routes++;

        if (canReachRightBoundary)
            routes++;

        return routes;
    }

    static bool IsCornered(bool[,] board, Vector2Int start, int width, int height, int escapeRouteCount, int reachableTiles)
    {
        int openAdjacent = 0;

        for (int i = 0; i < Directions.Length; i++)
        {
            Vector2Int next = start + Directions[i];
            if (!IsInBounds(next, width, height))
                continue;

            if (!board[next.x, next.y])
                openAdjacent++;
        }

        return openAdjacent <= 1 || (escapeRouteCount == 0 && reachableTiles <= 2);
    }

    static float CalculateDangerScore(int width, int reachableTiles, int adjacentBlocks, int escapeRouteCount, bool isCornered)
    {
        float reachableRatio = width > 0 ? Mathf.Clamp01((float)reachableTiles / width) : 0f;
        float confinement = 1f - reachableRatio;
        float adjacencyPressure = adjacentBlocks / 4f;
        float escapePenalty = escapeRouteCount > 0 ? 0f : 1f;
        float cornerPenalty = isCornered ? 1f : 0f;

        float weighted = (confinement * 0.45f) + (adjacencyPressure * 0.30f) + (escapePenalty * 0.15f) + (cornerPenalty * 0.10f);
        return Mathf.Clamp01(weighted) * 10f;
    }

    static int CountRun(bool[,] board, Vector2Int start, int width, int height, Vector2Int direction)
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

    static int CountAdjacentBlocks(bool[,] board, Vector2Int pos, int width, int height)
    {
        int count = 0;

        for (int i = 0; i < Directions.Length; i++)
        {
            Vector2Int next = pos + Directions[i];

            if (!IsInBounds(next, width, height))
                continue;

            if (board[next.x, next.y])
                count++;
        }

        return count;
    }

    static bool IsInBounds(Vector2Int pos, int width, int height)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
}