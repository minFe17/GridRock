/// <summary>
/// AI가 특정 Goal을 선택했을 때의 성공/실패 결과를 기록하기 위한 학습 로그용 데이터 구조
/// </summary>
public readonly struct AIStrategyRecord
{
    public readonly EAIGoalType Goal;
    public readonly bool Success;
    public readonly float TotalScore;

    public AIStrategyRecord(EAIGoalType goal, bool success, float totalScore)
    {
        Goal = goal;
        Success = success;
        TotalScore = totalScore;
    }
}