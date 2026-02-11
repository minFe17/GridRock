/// <summary>
/// AI가 실제로 선택하여 실행할 수 있는 하나의 행동 단위 인터페이스
/// </summary>
public interface IAIAction
{
    EAIActionTagType ActionTag { get; }

    bool CanExecute(in AISimulationState sim);
    void Execute(in AIActionContext context);
}