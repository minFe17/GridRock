using UnityEngine;

/// <summary>
/// 플레이어의 좌/우 이동 성향을 점수로 예측하는 클래스
/// - 실제 행동을 결정하지 않음
/// </summary>
public static class MovementPredictor
{
    const float NEG_INF = -9999f;   // 이동 불가 방향 배제용 점수
    const float DIR_BONUS = 2.0f;   // 최근 이동 방향 관성 보너스
    const float HOLD_MAX = 1.5f;    // 방향 유지 시간 최대 반영치

    public static PredictionResult Predict(in PredictionInput input)
    {
        // 좌/우 기본 점수 (이동 불가 방향은 즉시 배제)
        float leftScore = input.CanMoveLeft ? 0f : NEG_INF;
        float rightScore = input.CanMoveRight ? 0f : NEG_INF;

        // 최근 이동 방향 + 유지 시간 기반 관성 보정
        ApplyMovementHistory(ref leftScore, ref rightScore, input);

        // 방향별 맥락 정보 반영 (탈출 / 위험 / 보상)
        ApplyContextBias(ref leftScore, ref rightScore, input);

        // 점수와 방향별 맥락 요약 결과 반환
        return new PredictionResult(leftScore, rightScore, input.CanEscapeLeft || input.CanEscapeRight, input.NearDangerLeft || input.NearDangerRight, input.NearTetrisLeft || input.NearTetrisRight);
    }

    // 최근 이동 방향과 유지 시간에 따른 관성 점수 적용
    static void ApplyMovementHistory(ref float leftScore, ref float rightScore, in PredictionInput input)
    {
        float holdBonus = Mathf.Clamp(input.LastMoveDuration, 0f, HOLD_MAX);

        if (input.LastMoveDir == EMoveDirectionType.Left)
            leftScore += DIR_BONUS + holdBonus;
        else if (input.LastMoveDir == EMoveDirectionType.Right)
            rightScore += DIR_BONUS + holdBonus;
    }

    // 방향별 환경 맥락에 따른 점수 보정
    static void ApplyContextBias(ref float leftScore, ref float rightScore, in PredictionInput input)
    {
        // 탈출 가능 방향은 선호
        if (input.CanEscapeLeft)
            leftScore += 3.0f;
        if (input.CanEscapeRight)
            rightScore += 3.0f;

        // 위험 요소는 감점 (AI가 플레이어를 몰아넣을 수 있게 하기 위함)
        if (input.NearDangerLeft)
            leftScore -= 2.5f;
        if (input.NearDangerRight)
            rightScore -= 2.5f;

        // 테트리스 완성에 기여하는 방향은 가점
        if (input.NearTetrisLeft)
            leftScore += 2.0f;
        if (input.NearTetrisRight)
            rightScore += 2.0f;
    }
}