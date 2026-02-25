/// <summary>
/// AI가 선택 가능한 블록 후보 정보
/// </summary>
public readonly struct BlockOptionContext
{
    public readonly EBlockType BlockType;

    public BlockOptionContext(EBlockType blockType)
    {
        BlockType = blockType;
    }
}