using System.Collections.Generic;

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
        return new AIContext(_player, _grid, _availableBlocks, _activeBlock, _aiState);
    }
}