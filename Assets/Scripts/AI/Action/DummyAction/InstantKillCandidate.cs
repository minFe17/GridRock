/// <summary>
/// 특정 조건에서 플레이어를 즉시 패배로 몰기 위한 고위험, 고압박 행동 후보
/// </summary>
public sealed class InstantKillActionCandidate : IAIActionCandidate
{
    EAIActionTagType IAIActionCandidate.ActionTag => EAIActionTagType.InstantKill;

    float IAIActionCandidate.PressureCost => 2.5f;

    IAIAction IAIActionCandidate.Action => throw new System.NotImplementedException();
}