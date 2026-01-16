using UnityEngine;

public static class MovementPredictor
{
    const float NEG_INF = -9999f;
    const float DIR_BONUS = 2.0f;
    const float MAX_SCORE_GAP = 4.0f;

    static void ApplyMovementHistory(ref float leftScore, ref float rightScore, in PredictionInput input)
    {
        if (input.LastMoveDir == EMoveDirectionType.Left)
            leftScore += DIR_BONUS + Mathf.Clamp(input.LastMoveDuration, 0f, 1.5f);
        else if (input.LastMoveDir == EMoveDirectionType.Right)
            rightScore += DIR_BONUS + Mathf.Clamp(input.LastMoveDuration, 0f, 1.5f);
    }

    static void ApplySituationalBias(ref float leftScore, ref float rightScore, in PredictionInput input)
    {
        if (input.CanEscape)
        {
            leftScore += 3.0f;
            rightScore += 3.0f;
        }

        if (input.NearDanger)
        {
            leftScore += 2.5f;
            rightScore += 2.5f;
        }

        if (input.NearTetris)
        {
            leftScore += 2.0f;
            rightScore += 2.0f;
        }
    }

    static void PickBestDirection(float leftScore, float rightScore, out EMoveDirectionType bestDir, out float bestScore, out float secondScore)
    {
        if (leftScore >= rightScore)
        {
            bestDir = EMoveDirectionType.Left;
            bestScore = leftScore;
            secondScore = rightScore;
        }
        else
        {
            bestDir = EMoveDirectionType.Right;
            bestScore = rightScore;
            secondScore = leftScore;
        }
    }

    static Vector2 DirToVector(EMoveDirectionType dir)
    {
        return dir == EMoveDirectionType.Left ? Vector2.left : Vector2.right;
    }

    public static PredictionResult Predict(in PredictionInput input)
    {
        // 초기 점수 테이블 생성 & 이동 불가 처리
        float leftScore = input.CanMoveLeft ? 0f : NEG_INF;
        float rightScore = input.CanMoveRight ? 0f : NEG_INF;

        // 관성 & 유지 시간 보정
        ApplyMovementHistory(ref leftScore, ref rightScore, input);

        // 상황 기반 점수 편향
        ApplySituationalBias(ref leftScore, ref rightScore, input);

        // 최고 점수 방향 선택
        PickBestDirection(leftScore, rightScore, out EMoveDirectionType bestDir, out float bestScore, out float secondScore);

        // 신뢰도 계산
        float dirConfidence = Mathf.Clamp01((bestScore - secondScore) / MAX_SCORE_GAP);

        // 예상 위치
        Vector2 expectedPos = input.PlayerPos + DirToVector(bestDir);

        // 결과 반환
        return new PredictionResult(bestDir, expectedPos, dirConfidence, input.CanEscape, input.NearDanger, input.NearTetris, dirConfidence);
    }
}
