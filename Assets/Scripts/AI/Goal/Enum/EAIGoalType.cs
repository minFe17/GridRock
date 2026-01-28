/// <summary>
/// AI가 현재 추구하는 상위 목적(의도)
/// 행동이 아니라 '왜 이 행동을 하는가'에 해당
/// </summary>
public enum EAIGoalType
{
    None,
    KillNow,        // 즉시 압사 / 사망 유도
    TrapPlayer,     // 플레이어를 가두기
    ForceMistake,   // 실수 유도 (불리한 선택 강제)
    ApplyPressure,  // 압박 유지 (공간 축소, 시간 압박)
    Max,
}