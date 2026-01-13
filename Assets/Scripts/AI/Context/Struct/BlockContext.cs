using UnityEngine;

public readonly struct BlockContext
{
    public readonly Vector2Int SpawnGridPosition;  // 블록 생성 예정 위치
    public readonly int BlockType;                 // 블록 종류 (I, O, T 등)
    public readonly int Rotation;                  // 현재 회전 상태 (0~3)
    public readonly float FallSpeed;               // 현재 낙하 속도
    public readonly bool CanRotate;                // 회전 가능 여부

    public BlockContext(Vector2Int spawnGridPosition, int blockType, int rotation, float fallSpeed, bool canRotate)
    {
        SpawnGridPosition = spawnGridPosition;
        BlockType = blockType;
        Rotation = rotation;
        FallSpeed = fallSpeed;
        CanRotate = canRotate;
    }
}