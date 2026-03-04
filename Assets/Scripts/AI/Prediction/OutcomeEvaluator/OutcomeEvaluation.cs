/// <summary>
/// 시뮬레이션된 상태를 여러 평가 기준으로 수치화한 결과를 담는 구조체
/// </summary>
public readonly struct OutcomeEvaluation
{
    public readonly EAIGoalType Goal;

    public readonly float SurvivalScore;
    public readonly float EscapeScore;
    public readonly float DangerScore;
    public readonly float TetrisScore;

    public readonly float TotalScore;

    public OutcomeEvaluation(EAIGoalType goal, float survival, float escape, float danger, float tetris, float totalScore)
    {
        Goal = goal;

        SurvivalScore = survival;
        EscapeScore = escape;
        DangerScore = danger;
        TetrisScore = tetris;

        TotalScore = totalScore;
    }
}