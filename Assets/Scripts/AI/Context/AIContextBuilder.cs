using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 게임 월드 상태를 수집하여 AI가 판단에 사용할 AIContext를 생성하는 역할
/// </summary>
public class AIContextBuilder
{
    // 싱글턴
    PlayerContext _player;
    GridContext _grid;
    IReadOnlyList<BlockOptionContext> _availableBlocks;
    BlockContext? _activeBlock;
    AIStateContext _aiState;

    public PlayerContext PlayerContext { set { _player = value; } }
    public GridContext GridContext { set { _grid = value; } }
    public IReadOnlyList<BlockOptionContext> AvailableBlocks { set { _availableBlocks = value; } }
    public BlockContext ActiveBlock { set { _activeBlock = value; } }
    public AIStateContext AIStateContext { set { _aiState = value; } }

    public AIContext Build()
    {
        // 임시로 1,1
        int holeCount = CalculateHoleCount(1, 1);
        _grid = new GridContext(_grid.Occupancy, holeCount);
        return new AIContext(_player, _grid, _availableBlocks, _activeBlock, _aiState);
    }

    int CalculateHoleCount(int rangeX, int rangeY)
    {
        Vector2 playerPos = _player.GridPosition;
        int holes = 0;
        bool[,] board = _grid.Occupancy;

        int startX = Mathf.Max(0, (int)playerPos.x - rangeX);
        int endX = Mathf.Min(board.GetLength(0) - 1, (int)playerPos.x + rangeX);

        int startY = Mathf.Max(0, (int)playerPos.y - rangeY);
        int endY = Mathf.Min(board.GetLength(1) - 1, (int)playerPos.y + rangeY);

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (!board[x, y])
                    holes++;
            }
        }
        return holes;
    }
}