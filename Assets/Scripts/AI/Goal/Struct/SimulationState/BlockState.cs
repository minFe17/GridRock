using UnityEngine;

public readonly struct BlockState
{
    public readonly EBlockType BlockType;      // 블록 종류
    public readonly Vector2 Position;          // 현재 위치
    public readonly int Rotation;              // 현재 회전 상태
    public readonly bool CanRotate;            // 회전 가능 여부
    public readonly float PressureScore;       // 주변 압박 정도

    public BlockState(EBlockType blockType, Vector2 position, int rotation, bool canRotate, float pressureScore)
    {
        BlockType = blockType;
        Position = position;
        Rotation = rotation;
        CanRotate = canRotate;
        PressureScore = pressureScore;
    }
}