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