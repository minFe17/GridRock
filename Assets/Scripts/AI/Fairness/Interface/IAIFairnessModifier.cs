/// <summary>
/// 상황 기반 페어너스 보정 인터페이스
/// 점수 조정용 훅
/// </summary>
public interface IAIFairnessModifier
{
    float Modify(float baseScore, in PredictionResult prediction, in OutcomeEvaluation evaluation);
}