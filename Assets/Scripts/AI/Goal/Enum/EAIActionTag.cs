/// <summary>
/// AI 행동의 의도적 분류 태그
/// 방해/압박/즉사 등 목적 제한에 사용됨
/// </summary>
public enum EAIActionTag
{
    None,

    // 방해 계열
    BlockEscape,       // 탈출 경로 차단
    CreateHole,        // 일부러 구멍 생성
    CreateDanger,      // 위험 블록 생성
    ApplyPressure,     // 상단 압박
    InstantKill,       // 즉사 유도

    Max,
}