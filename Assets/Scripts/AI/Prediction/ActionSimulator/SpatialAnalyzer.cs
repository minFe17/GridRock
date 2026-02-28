using System.Collections.Generic;
using UnityEngine;

public static class SpatialAnalyzer
{
    public static SpatialMetrics Analyze(bool[,] board, Vector2 playerPos)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        bool[,] visited = new bool[width, height];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Vector2Int start = new Vector2Int((int)playerPos.x, (int)playerPos.y);

        // 시작 위치가 막혀 있으면 탈출 0
        if (board[start.x, start.y])
            return new SpatialMetrics(0, 0, 0, 0, 0);

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        int reachableCount = 0;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            reachableCount++;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                    continue;

                if (visited[next.x, next.y])
                    continue;

                if (board[next.x, next.y])
                    continue;

                visited[next.x, next.y] = true;
                queue.Enqueue(next);
            }
        }

        return new SpatialMetrics(reachableTileCount: reachableCount, regionSize: reachableCount, escapePathLength: reachableCount, chokePointCount: 0, adjacentBlockCount: 0);
    }
}