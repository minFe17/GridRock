
/// <summary>
/// AI 판단에 필요한 모든 정보를 묶은 최상위 컨텍스트
/// - 한 프레임 기준 AI가 바라보는 '세계 스냅샷'
/// </summary>
public readonly struct AIContext
{
    public readonly PlayerContext Player;
    public readonly GridContext Grid;
    public readonly BlockContext Block;
    public readonly AIStateContext AIState;

    public AIContext(PlayerContext player, GridContext grid, BlockContext block, AIStateContext aIState)
    {
        Player = player;
        Grid = grid;
        Block = block;
        AIState = aIState;
    }
}