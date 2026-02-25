public readonly struct BlockContext
{
    public readonly EBlockType BlockType;                 // 블록 종류 (I, O, T 등)
    public readonly int Rotation;                  // 현재 회전 상태 (0~3)

    public BlockContext(EBlockType blockType, int rotation)
    {
        BlockType = blockType;
        Rotation = rotation;
    }
}