/// <summary>
/// AI 페어니스 전반 설정값 묶음
/// AI가 볼 수 있는 정보 범위와 예측 한계를 정의
/// </summary>
public readonly struct AIFairnessConfig
{
    public readonly float PredictionInterval; // 예측 갱신 주기
    public readonly int MaxSimulationDepth;   // 최대 시뮬레이션 깊이
}