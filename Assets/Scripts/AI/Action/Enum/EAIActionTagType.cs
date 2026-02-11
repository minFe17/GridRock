/// <summary>
/// AI가 선택할 수 있는 행동의 의미를 정의한 열거형
/// </summary>
public enum EAIActionTagType
{
    None,

    // 방해 계열
    BlockEscape,       // 탈출 경로 차단
    CreateHole,        // 일부러 구멍 생성
    CreateDanger,      // 위험 블록 생성
    ApplyPressure,     // 상단 압박
    InstantKill,       // 즉사 유도
}