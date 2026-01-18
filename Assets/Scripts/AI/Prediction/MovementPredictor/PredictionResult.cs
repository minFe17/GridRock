/// <summary>
/// 좌우 이동에 대한 상대적 선호 점수와 현재 상황의 핵심 맥락을 담은 예측 결과 데이터
/// </summary>
public readonly struct PredictionResult
{
    public readonly float LeftScore;      // 왼쪽 이동 선호도
    public readonly float RightScore;     // 오른쪽 이동 선호도

    public readonly bool HasEscapeRoute;  // 탈출 가능성 존재 여부
    public readonly bool HasDanger;       // 위험 요소 존재 여부
    public readonly bool HasTetris;       // 보상 요소 존재 여부

    public PredictionResult(float leftScore, float rightScore, bool hasEscapeRoute, bool hasDanger, bool hasTetris)
    {
        LeftScore = leftScore;
        RightScore = rightScore;
        HasEscapeRoute = hasEscapeRoute;
        HasDanger = hasDanger;
        HasTetris = hasTetris;
    }
}