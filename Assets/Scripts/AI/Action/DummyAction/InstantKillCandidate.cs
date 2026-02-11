/// <summary>
/// 특정 조건에서 플레이어를 즉시 패배로 몰기 위한 고위험, 고압박 행동 후보
/// </summary>
public sealed class InstantKillActionCandidate : IAIActionCandidate
{
    public EAIActionTagType ActionTag => EAIActionTagType.InstantKill;

    public float PressureCost => 2.5f;
}