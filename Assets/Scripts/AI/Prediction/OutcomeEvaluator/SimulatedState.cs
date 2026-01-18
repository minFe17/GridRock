using UnityEngine;

/// <summary>
/// AI가 특정 행동을 가정했을 때 도달하는 가상의 결과 상태를 표현하는 구조체
/// 실제 월드 상태가 아니라, 예측/시뮬레이션용 상태 스냅샷
/// </summary>
public readonly struct SimulatedState
{
    public readonly Vector2 Position;
    public readonly bool IsTrapped;        // 사방 막힘
    public readonly bool HasEscapeRoute;   // 빠져나갈 경로 존재
    public readonly bool NearDanger;       // 압사/낙하 위험
    public readonly bool HelpsTetris;      // 테트리스 완성에 기여

    public SimulatedState(Vector2 position, bool isTrapped, bool hasEscapeRoute, bool nearDanger, bool helpsTetris)
    {
        Position = position;
        IsTrapped = isTrapped;
        HasEscapeRoute = hasEscapeRoute;
        NearDanger = nearDanger;
        HelpsTetris = helpsTetris;
    }
}