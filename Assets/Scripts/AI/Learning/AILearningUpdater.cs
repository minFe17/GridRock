/// <summary>
/// 학습 이벤트를 받아 가중치들을 갱신
/// </summary>
public class AILearningUpdater
{
    readonly AIGoalWeightTable _goalWeights;
    readonly AIActionPriorityTable _actionPriorities;
    readonly AIDirectionBias _directionBias;

    public AILearningUpdater(AIGoalWeightTable goalWeights, AIActionPriorityTable actionPriorities, AIDirectionBias directionBias)
    {
        _goalWeights = goalWeights;
        _actionPriorities = actionPriorities;
        _directionBias = directionBias;
    }

    public void Apply(in AILearningEvent learningEvent)
    {
        // 공격 성공
        if (learningEvent.AttackSucceeded)
        {
            _goalWeights.Adjust(learningEvent.Goal, +0.1f);
            _actionPriorities.Adjust(learningEvent.ActionTag, +0.15f);
        }
        else
            _actionPriorities.Adjust(learningEvent.ActionTag, -0.1f);

        // 예측 실패
        if (!learningEvent.PredictionMatched)
            _actionPriorities.Adjust(learningEvent.ActionTag, -0.05f);

        // 테트리스 방해 실패
        if (!learningEvent.TetrisBlocked)
            _actionPriorities.Adjust(learningEvent.ActionTag, -0.08f);

        // 자주 탈출한 방향 → 해당 방향 방해 신뢰도 감소
        _directionBias.Adjust(learningEvent.PlayerEscape, -0.05f);
    }
}