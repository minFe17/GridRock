/// <summary>
/// AI 행동 실행 전 페어니스 규칙을 검사하는 인터페이스
/// </summary>
public interface IAIFairnessFilter
{
    bool CanExecuteAction(EAIActionTag actionTag, float pressureCost);
}