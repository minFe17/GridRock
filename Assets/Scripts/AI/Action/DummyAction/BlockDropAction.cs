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
        Debug.Log($"[AI] Use DraftSlot:{_blockSlot} Block:{_blockType}, Rotation:{_rotation}, Cell:{_dropCell}");
    }
}