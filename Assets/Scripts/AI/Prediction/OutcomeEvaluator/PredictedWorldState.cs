using UnityEngine;

/// <summary>
/// AI가 특정 행동을 가정했을 때 도달하는 가상의 결과 상태를 표현하는 구조체
/// 실제 월드 상태가 아니라, 예측/시뮬레이션용 상태 스냅샷
/// </summary>
public readonly struct PredictedWorldState
{
    public readonly Vector2 Position;

    // 공간 정보
    public readonly SpatialMetrics Spatial;

    // 미래 위험
    public readonly float FutureTrapRisk;     // 0~1 사이

    public PredictedWorldState(Vector2 position, SpatialMetrics spatial, float futureTrapRisk)
    {
        Position = position;
        Spatial = spatial;
        FutureTrapRisk = futureTrapRisk;
    }
}