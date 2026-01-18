/// <summary>
/// 시뮬레이션된 상태를 여러 평가 기준으로 수치화한 결과를 담는 구조체
/// 각 항목 점수와 이를 종합한 최종 점수를 함께 제공하여
/// AI가 여러 행동 결과를 서로 비교, 선택할 수 있도록 함
/// </summary>
public readonly struct OutcomeEvaluation
{
    public readonly float SurvivalScore;      // 살아남을 가능성
    public readonly float EscapeScore;        // 갇힘 해소 정도
    public readonly float TetrisScore;        // 테트리스 기여도
    public readonly float DangerScore;        // 위험도 (높을수록 나쁨)

    public readonly float TotalScore;         // 종합 점수

    public OutcomeEvaluation(float survivalScore, float escapeScore, float tetrisScore, float dangerScore)
    {
        SurvivalScore = survivalScore;
        EscapeScore = escapeScore;
        TetrisScore = tetrisScore;
        DangerScore = dangerScore;

        // 위험은 감점
        TotalScore = survivalScore + escapeScore + tetrisScore - dangerScore;
    }
}