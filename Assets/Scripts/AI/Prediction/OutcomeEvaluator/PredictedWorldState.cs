using UnityEngine;

/// <summary>
/// AI가 특정 행동을 시뮬레이션한 뒤 예측된 월드 상태를 표현
/// </summary>
public readonly struct PredictedWorldState
{
    public readonly Vector2 Position;

    // 블록 적용 전/후 공간 분석 결과
    public readonly SpatialMetrics SpatialBefore;
    public readonly SpatialMetrics SpatialAfter;

    // 도달 가능한 타일 감소량 (Before - After)
    public readonly int ReachableDelta;

    // 미래 위험
    public readonly float FutureTrapRisk;

    public PredictedWorldState(Vector2 position, SpatialMetrics spatialBefore, SpatialMetrics spatialAfter, float futureTrapRisk)
    {
        Position = position;
        SpatialBefore = spatialBefore;
        SpatialAfter = spatialAfter;
        ReachableDelta = spatialBefore.ReachableTileCount - spatialAfter.ReachableTileCount;
        FutureTrapRisk = futureTrapRisk;
    }
}