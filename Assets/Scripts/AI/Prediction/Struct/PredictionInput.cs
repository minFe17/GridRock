using UnityEngine;

public readonly struct PredictionInput
{
    public readonly Vector2 PlayerPos;
    public readonly EMoveDirectionType LastMoveDir;
    public readonly float LastMoveDuration;

    public readonly bool CanMoveLeft;
    public readonly bool CanMoveRight;

    public readonly bool CanJump;
    public readonly bool IsInAir;

    public readonly bool CanEscape;
    public readonly bool NearDanger;
    public readonly bool NearTetris;

    public PredictionInput(Vector2 playerPos, EMoveDirectionType lastMoveDir, float lastMoveDuration, bool canMoveLeft, bool canMoveRight, bool canJump, bool isInAir, bool canEscape, bool nearDanger, bool nearTetris)
    {
        PlayerPos = playerPos;
        LastMoveDir = lastMoveDir;
        LastMoveDuration = lastMoveDuration;
        CanMoveLeft = canMoveLeft;
        CanMoveRight = canMoveRight;
        CanJump = canJump;
        IsInAir = isInAir;
        CanEscape = canEscape;
        NearDanger = nearDanger;
        NearTetris = nearTetris;
    }
}
