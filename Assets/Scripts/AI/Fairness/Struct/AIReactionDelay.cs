/// <summary>
/// AI 반응 지연 정보
/// 인간 반응 속도 보정용
/// </summary>
public readonly struct AIReactionDelay
{
    public readonly float ReactionTime; // 반응 지연 시간

    public AIReactionDelay(float reactionTime)
    {
        ReactionTime = reactionTime;
    }
}