using System.Collections.Generic;
using UnityEngine;

public static class SpatialAnalyzer
{
    public static SpatialMetrics Analyze(bool[,] board, Vector2 playerPos)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        int regionSize = CountEmptyTiles(board, width, height);

        Vector2Int start = new Vector2Int((int)playerPos.x, (int)playerPos.y);
        bool isStartInBounds = start.x >= 0 && start.x < width && start.y >= 0 && start.y < height;

        int reachableCount = 0;

        // reachableTileCount는 시작 위치 기준 BFS 결과만 사용한다.
        if (isStartInBounds && !board[start.x, start.y])
        {
            bool[,] visited = new bool[width, height];
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            queue.Enqueue(start);
            visited[start.x, start.y] = true;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                reachableCount++;

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int next = current + dir;

                    if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                        continue;

                    if (visited[next.x, next.y] || board[next.x, next.y])
                        continue;

                    visited[next.x, next.y] = true;
                    queue.Enqueue(next);
                }
            }
        }

        // regionSize는 보드 전체 빈칸 수로, reachableTileCount와 독립적으로 계산한다.
        return new SpatialMetrics(reachableCount, regionSize);
    }

    private static int CountEmptyTiles(bool[,] board, int width, int height)
    {
        int emptyTileCount = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!board[x, y])
                    emptyTileCount++;
            }
        }

        return emptyTileCount;
    }
}
