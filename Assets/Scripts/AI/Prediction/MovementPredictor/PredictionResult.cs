/// <summary>
/// 좌우 이동에 대한 상대적 선호 점수와 현재 상황의 핵심 맥락을 담은 예측 결과 데이터
/// </summary>
public readonly struct PredictionResult
{
    public readonly float LeftScore;      // 왼쪽 이동 선호도
    public readonly float RightScore;     // 오른쪽 이동 선호도

    public PredictionResult(float leftScore, float rightScore)
    {
        LeftScore = leftScore;
        RightScore = rightScore;
    }
}