/// <summary>
/// 현재 AI 목적(EAIGoalType)을 계속 유지할지
/// 또는 즉시 종료하고 다른 목적로 전환해야 하는지를 판단하는 인터페이스
/// </summary>
public interface IAIGoalTermination
{
    bool ShouldTerminate(EAIGoalType goal, in PredictionResult prediction, in OutcomeEvaluation evaluation);
}