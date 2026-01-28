/// <summary>
/// AI 의사결정 단계에서 사용하는 종합 시뮬레이션 상태
/// 예측 결과와 평가 결과를 하나로 묶어 Goal 선택 및 Action 판단의 기준으로 사용됨
/// </summary>
public readonly struct AISimulationState
{
    // --- 예측 정보 ---
    public readonly bool HasEscapeRoute;    // 탈출 경로 존재 여부
    public readonly bool HasDanger;         // 즉각적 위험 존재 여부

    // --- 평가 점수 ---
    public readonly float SurvivalScore;    // 생존 가능성 점수
    public readonly float EscapeScore;      // 탈출 가능성 점수
    public readonly float DangerScore;      // 위험도 점수
    public readonly float TetrisScore;      // 테트리스 기여 점수

    // --- 종합 판단 ---
    public readonly float TotalScore;       // 최종 종합 점수

    public AISimulationState(in PredictionResult prediction, in OutcomeEvaluation evaluation)
    {
        HasEscapeRoute = prediction.HasEscapeRoute;
        HasDanger = prediction.HasDanger;

        SurvivalScore = evaluation.SurvivalScore;
        EscapeScore = evaluation.EscapeScore;
        DangerScore = evaluation.DangerScore;
        TetrisScore = evaluation.TetrisScore;

        TotalScore = evaluation.TotalScore;
    }
}