/// <summary>
/// 테트리스 시스템이 AI에게 제공하는 하나의 행동 선택지에 대한 추상 표현
/// </summary>
public interface IAIActionCandidate
{
    EAIActionTagType ActionTag { get; }   // 이 선택지가 유발하는 AI 행동 의미
    float PressureCost { get; }       // 페어니스용 압박 비용
    IAIAction Action { get; }
}