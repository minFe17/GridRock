/// <summary>
/// 플레이어에게 지속적인 압박을 가하기 위한 상단 압박 계열 행동 후보
/// </summary>
public sealed class ApplyPressureActionCandidate : IAIActionCandidate
{
    EAIActionTagType IAIActionCandidate.ActionTag => EAIActionTagType.ApplyPressure;

    float IAIActionCandidate.PressureCost => 0.8f;

    IAIAction IAIActionCandidate.Action => throw new System.NotImplementedException();
}