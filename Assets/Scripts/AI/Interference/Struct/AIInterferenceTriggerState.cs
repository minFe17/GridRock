/// <summary>
/// 플레이어가 유리한 상태로 진입 중인지 판단하기 위한 방해 전용 트리거 정보
/// </summary>
public readonly struct AIInterferenceTriggerState
{
    public readonly bool IsNearLineClear;   // 줄 제거 직전
    public readonly bool IsNearTetris;      // 테트리스 직전
    public readonly bool IsHighStack;       // 상단 압박 상태
    public readonly bool IsCommitMoment;    // 이미 선택을 되돌리기 어려운 순간

    public AIInterferenceTriggerState(bool isNearLineClear, bool isNearTetris, bool isHighStack, bool isCommitMoment)
    {
        IsNearLineClear = isNearLineClear;
        IsNearTetris = isNearTetris;
        IsHighStack = isHighStack;
        IsCommitMoment = isCommitMoment;
    }
}