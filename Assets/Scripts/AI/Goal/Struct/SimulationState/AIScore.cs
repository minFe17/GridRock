public readonly struct AIScore
{
    public readonly float SurvivalScore;    // 생존 가능성 점수
    public readonly float EscapeScore;      // 탈출 가능성 점수
    public readonly float DangerScore;      // 위험도 점수
    public readonly float TetrisScore;      // 테트리스 기여 점수
    public readonly float TotalScore;       // 최종 종합 점수

    public AIScore(float survivalScore, float escapeScore, float tetrisScore, float dangerScore)
    {
        SurvivalScore = survivalScore;
        EscapeScore = escapeScore;
        TetrisScore = tetrisScore;
        DangerScore = dangerScore;
        TotalScore = survivalScore + escapeScore + tetrisScore - dangerScore;
    }

    public AIScore(in OutcomeEvaluation evaluation)
    {
        SurvivalScore = evaluation.SurvivalScore;
        EscapeScore = evaluation.EscapeScore;
        TetrisScore = evaluation.TetrisScore;
        DangerScore = evaluation.DangerScore;
        TotalScore = evaluation.TotalScore;
    }
}