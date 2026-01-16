using UnityEngine;

public readonly struct PredictionResult
{
    public readonly EMoveDirectionType ExpectedDir;      // 가장 유력한 이동 방향
    public readonly Vector2 ExpectedPos;                 // 해당 방향으로 이동했을 때 예상 위치

    public readonly float DirConfidence;                 // 방향 신뢰도 (0~1)

    public readonly bool CanEscape;                    // 출구로 향하는 중인가
    public readonly bool EvadingDanger;                  // 위험 회피 행동인가
    public readonly bool GoingToTetris;                  // 보상(테트리스 등)을 노리는가

    public readonly float OverallConfidence;             // 전체 예측 신뢰도

    public PredictionResult(EMoveDirectionType expectedDir, Vector2 expectedPos, float dirConfidence, bool canEscape, bool evadingDanger, bool goingToTetris, float overallConfidence)
    {
        ExpectedDir = expectedDir;
        ExpectedPos = expectedPos;
        DirConfidence = dirConfidence;
        CanEscape = canEscape;
        EvadingDanger = evadingDanger;
        GoingToTetris = goingToTetris;
        OverallConfidence = overallConfidence;
    }
}