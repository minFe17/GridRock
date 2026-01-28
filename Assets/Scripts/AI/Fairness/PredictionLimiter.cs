/// <summary>
/// AI 예측 갱신 빈도 제한
/// 매 프레임 완벽한 판단 방지
/// </summary>
public class PredictionLimiter
{
    private float lastPredictionTime;   // 마지막 예측 시각
    private readonly float interval;    // 최소 예측 간격

    public PredictionLimiter(float interval)
    {
        this.interval = interval;
        lastPredictionTime = -interval;
    }

    public bool CanUpdate(float currentTime)
    {
        if (currentTime - lastPredictionTime < interval)
            return false;

        lastPredictionTime = currentTime;
        return true;
    }
}