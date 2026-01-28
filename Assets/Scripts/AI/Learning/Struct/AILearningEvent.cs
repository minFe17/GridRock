/// <summary>
/// AI가 특정 패턴을 사용한 후의 결과 기록
/// 학습은 이 이벤트를 누적 분석
/// </summary>
public readonly struct AILearningEvent
{
    public readonly EAIGoalType Goal;                 // 당시 AI 목적
    public readonly EAIActionTag ActionTag;           // 사용한 행동 패턴
    public readonly EMoveDirectionType PlayerEscape;  // 플레이어 탈출 방향

    public readonly bool AttackSucceeded;             // 사망 / 압사 성공 여부
    public readonly bool PredictionMatched;           // 예측과 실제 일치 여부
    public readonly bool TetrisBlocked;               // 테트리스 방해 성공 여부

    public AILearningEvent(EAIGoalType goal, EAIActionTag actionTag, EMoveDirectionType playerEscape, bool attackSucceeded, bool predictionMatched, bool tetrisBlocked)
    {
        Goal = goal;
        ActionTag = actionTag;
        PlayerEscape = playerEscape;
        AttackSucceeded = attackSucceeded;
        PredictionMatched = predictionMatched;
        TetrisBlocked = tetrisBlocked;
    }
}