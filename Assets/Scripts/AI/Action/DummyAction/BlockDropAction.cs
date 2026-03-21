using UnityEngine;

/// <summary>
/// 실제 블록 드롭 행동
/// </summary>
sealed class BlockDropAction : IAIAction
{
    readonly EBlockType _blockType;
    readonly int _rotation;
    readonly Vector2Int _dropCell;
    readonly int _blockSlot;

    public EAIActionTagType ActionTag { get; }

    public EBlockType BlockType => _blockType;
    public int Rotation => _rotation;
    public Vector2Int DropCell => _dropCell;
    public int BlockSlot => _blockSlot;

    public BlockDropAction(EAIActionTagType actionTag, EBlockType blockType, int rotation, Vector2Int dropCell, int blockSlot)
    {
        ActionTag = actionTag;
        _blockType = blockType;
        _rotation = rotation;
        _dropCell = dropCell;
        _blockSlot = blockSlot;
    }

    public bool CanExecute(in AISimulationState sim)
    {
        return true;
    }

    public void Execute(in AIActionContext context)
    {
        BoardManager boardManager = BoardManager.Instance;
        if (boardManager == null)
            return;

        int slotNumber = _blockSlot + 1;
        bool selected = boardManager.TrySelectBlockSlot(slotNumber);

        if (!selected)
            return;

        BlockController block = boardManager.DropBlock;
        block.SetTarget(_dropCell, _rotation);
    }
}