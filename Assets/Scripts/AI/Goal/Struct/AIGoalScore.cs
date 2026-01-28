/// <summary>
/// 특정 AI 목적에 대해 현재 상황이 얼마나 적합한지를 수치로 표현한 결과 데이터
/// </summary>
public readonly struct AIGoalScore
{
    public EAIGoalType Goal { get; }    // 평가 대상이 되는 AI 목적 타입
    public float Score { get; }         // 해당 목적에 대한 최종 적합도 점수

    public AIGoalScore(EAIGoalType goal, float score)
    {
        Goal = goal;
        Score = score;
    }
}