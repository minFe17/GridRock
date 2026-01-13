public enum EAIActionType
{
    None,
    DropBlock,      // 특정 위치에 블록 낙하
    RotateBlock,    // 현재 블록 회전
    ClearLine,      // 라인 제거 시도 (테트리스 공격)
    Max
}