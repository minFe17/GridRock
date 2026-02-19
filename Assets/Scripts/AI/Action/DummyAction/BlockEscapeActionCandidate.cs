/// <summary>
/// 플레이어의 탈출 경로를 차단하려는 방해 목적의 행동 후보
/// </summary>
public sealed class BlockEscapeActionCandidate : IAIActionCandidate
{
    EAIActionTagType IAIActionCandidate.ActionTag => EAIActionTagType.BlockEscape;

    float IAIActionCandidate.PressureCost => 1.0f;

    IAIAction IAIActionCandidate.Action => throw new System.NotImplementedException();
}